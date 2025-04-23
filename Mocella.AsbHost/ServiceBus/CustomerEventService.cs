using System.Text.Json;
using Azure.Messaging.ServiceBus;
using FluentValidation.Results;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Options;
using Mocella.AsbHost.Configuration;
using Mocella.AsbHost.RequestObjects;
using Mocella.AsbHost.ResponseObjects;
using Mocella.AsbHost.Services;
using Mocella.AsbHost.Validators;

namespace Mocella.AsbHost.ServiceBus;

public class CustomerEventService : ServiceBusServiceBase<CustomerEventService>
{
    private readonly MocellaCustomerRequestValidator _MocellaCustomerRequestValidator;
    private readonly IMocellaCustomerService _MocellaCustomerService;

    public CustomerEventService(IOptions<AzureConfig> azureConfig,
        MocellaCustomerRequestValidator MocellaCustomerRequestValidator,
        IMocellaCustomerService MocellaCustomerService,
        IServiceBusFactory serviceBusFactory,
        ILogger<CustomerEventService> logger,
        TelemetryClient telemetryClient)
        : base(azureConfig.Value.ServiceBus!.Topics!.First(c => c.TopicName == AsbHostConstants.CustomerEventsTopicName), 
            serviceBusFactory,
            logger,
            telemetryClient)
    {
        _MocellaCustomerRequestValidator = MocellaCustomerRequestValidator;
        _MocellaCustomerService = MocellaCustomerService;
    }

    public override async Task<object> ProcessMessage(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        var customerRequest = JsonSerializer.Deserialize<MocellaCustomerRequest>(body,JsonSerializerOptions);
    
        EnumConverter.TryConvertStringToEnum(customerRequest!.Action, out RequestAction requestAction);
        
        var responseMessage = new MocellaCustomerResponse(customerEvent: null, 
            requestAction: requestAction, 
            requestDateUtc: customerRequest.RequestDateUtc, 
            requestSourceLastUpdatedDateUtc: customerRequest.SourceLastUpdatedDateUtc,
            responseDateUtc: DateTime.UtcNow, 
            AsbHostConstants.SourceSystemNetsuite);
    
        var validationResult = await _MocellaCustomerRequestValidator.ValidateAsync(customerRequest!);
        if(!validationResult.IsValid)
        {
            responseMessage.Errors = validationResult.Errors;
        }
        else
        {
            CustomerEvent response = null!;
            switch (requestAction)
            {
                case RequestAction.Add:
                    response = _MocellaCustomerService.Add(customerRequest.CustomerEvent);
                    break;
                case RequestAction.Update:
                    response = _MocellaCustomerService.Update(customerRequest.CustomerEvent);
                    break;
                case RequestAction.Delete:
                    response = _MocellaCustomerService.Delete(customerRequest.CustomerEvent);
                    break;
                default:
                    responseMessage.Errors.Add(new ValidationFailure("CustomerRequest.Action", $"Invalid Action specified: '{customerRequest.Action}'"));
                    break;
            }
            responseMessage.CustomerEvent = response;
            responseMessage.CustomerEvent.PortalId = response.PortalId!.Value;
        }

        return responseMessage;
    }
}