using System.Text.Json;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Options;
using Mocella.AsbHost.Configuration;
using Mocella.AsbHost.RequestObjects;

namespace Mocella.AsbHost.ServiceBus.Senders;

public class CustomerEventSender: ServiceBusSenderBase<CustomerEventSender, CustomerEvent>, ICustomerEventSender
{
    public CustomerEventSender(IOptions<AzureConfig> azureConfig,
        IServiceBusFactory serviceBusFactory,
        ILogger<CustomerEventSender> logger,
        TelemetryClient telemetryClient)
        : base(
            azureConfig.Value.ServiceBus!.Topics!.First(c => c.TopicName == AsbHostConstants.CustomerEventsTopicName),
            serviceBusFactory,
            logger,
            telemetryClient)
    {
    }

    public override Task CreateAndSendMessage(CustomerEvent eventDetails,
        RequestAction requestAction,
        DateTime sourceLastUpdatedDateUtc)
    {
        var requestDetails = new MocellaCustomerRequest(eventDetails,
            requestAction,
            DateTime.UtcNow,
            sourceLastUpdatedDateUtc,
            AsbHostConstants.SourceSystemPortal);

        TelemetryClient.TrackTrace("CustomerEventSender - CreateAndSendMessage - Serialize");
        var messageBody = JsonSerializer.Serialize(requestDetails, JsonSerializerOptions);
        SendMessageAsync(messageBody);
        
        return Task.CompletedTask;
    }
}