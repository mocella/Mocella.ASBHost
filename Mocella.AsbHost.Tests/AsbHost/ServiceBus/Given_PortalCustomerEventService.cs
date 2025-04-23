using Azure.Messaging.ServiceBus;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mocella.AsbHost.Configuration;
using Mocella.AsbHost.RequestObjects;
using Mocella.AsbHost.ResponseObjects;
using Mocella.AsbHost.ServiceBus;
using Mocella.AsbHost.Services;
using Mocella.AsbHost.Validators;

namespace Mocella.AsbHost.Tests.AsbHost.ServiceBus;

[Trait(Constants.TestCategory, Constants.UnitTestCategory)]
public class Given_PortalCustomerEventService 
{ 
    private readonly Mock<IOptions<AzureConfig>> _azureConfig;
    private readonly Mock<IMocellaCustomerService> _customerService;
    private readonly Mock<ILogger<CustomerEventService>> _logger;
    private readonly MocellaCustomerRequestValidator _validator;
    private readonly CustomerEventService _customerEventService;
    private readonly Mock<IServiceBusFactory> _serviceBusFactory;
    private readonly Mock<ILogger<MocellaCustomerService>> _customerServiceLogger;
    private readonly Mock<ServiceBusReceiver> _mockReceiver;

    public Given_PortalCustomerEventService() 
    {
        _azureConfig = new Mock<IOptions<AzureConfig>>();
        var azureConfig = TestData.AzureConfig();
        _azureConfig.Setup(ac => ac.Value).Returns(azureConfig);
        
        _validator = new MocellaCustomerRequestValidator();
        _logger = new Mock<ILogger<CustomerEventService>>();
        
        _customerServiceLogger = new Mock<ILogger<MocellaCustomerService>>();
        
        var fakeChannel = new FakeTelemetryChannel();
        var config = new TelemetryConfiguration
        {
            TelemetryChannel = fakeChannel,
            InstrumentationKey = "some key",
        };
        var client = new TelemetryClient(config);
        
        _customerService = new Mock<IMocellaCustomerService>();
        _serviceBusFactory = new Mock<IServiceBusFactory>();
        
        _mockReceiver = new Mock<ServiceBusReceiver>();
        _mockReceiver
            .Setup(receiver => receiver.CompleteMessageAsync(
                It.IsAny<ServiceBusReceivedMessage>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        _customerEventService = new CustomerEventService(_azureConfig.Object, _validator, _customerService.Object, _serviceBusFactory.Object, _logger.Object, client);
    }

    [Fact]
    public async Task Should_Process_Valid_Add_Message()
    {
        // arrange
        var customerEvent = TestData.CustomerEvent(portalId: Guid.NewGuid());
        var asbCustomerRequest = new MocellaCustomerRequest(customerEvent,
            RequestAction.Add,
            DateTime.UtcNow,
            DateTime.UtcNow,
            AsbHostConstants.SourceSystemPortal
        );
        
        var processArgs = SetupProcessMessageEventArgs(asbCustomerRequest);
        _customerService.Setup(cs => cs.Add(It.IsAny<CustomerEvent>()))
            .Returns(customerEvent)
            .Verifiable();

        // act
        var result = await _customerEventService.ProcessMessage(processArgs);
        
        // assert
        Assert.NotNull(result);
        Assert.True(result is MocellaCustomerResponse);
        var response = result as MocellaCustomerResponse;
        Assert.False(response!.HasErrors);
        _customerService.Verify();
    }

    private ProcessMessageEventArgs SetupProcessMessageEventArgs(MocellaCustomerRequest MocellaCustomerRequest)
    {
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: new BinaryData(MocellaCustomerRequest),
            messageId: "messageId",
            correlationId: Guid.NewGuid().ToString()
        );

        ProcessMessageEventArgs processArgs = new(
            message: message,
            receiver: _mockReceiver.Object,
            cancellationToken: CancellationToken.None);
        return processArgs;
    }
}