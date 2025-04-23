using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mocella.AsbHost.Configuration;
using Mocella.AsbHost.ServiceBus;

namespace Mocella.AsbHost.Tests.AsbHost.ServiceBus;

[Trait(Constants.TestCategory, Constants.UnitTestCategory)]
public class Given_ServiceBusFactory
{
    private readonly IServiceBusFactory _serviceBusFactory;
    private readonly Mock<IOptions<AzureConfig>> _azureConfigMock;
    private readonly Mock<ILogger<ServiceBusFactory>> _logger;
    private AzureConfig _azureConfig;

    public Given_ServiceBusFactory()
    {
        _azureConfigMock = new Mock<IOptions<AzureConfig>>();
        _azureConfig = TestData.AzureConfig();
        _azureConfigMock.Setup(ac => ac.Value).Returns(_azureConfig);

        _logger = new Mock<ILogger<ServiceBusFactory>>();

        _serviceBusFactory = new ServiceBusFactory(_azureConfigMock.Object, _logger.Object);
    }
    
    [Fact]
    public void Should_Use_Topic_ConnectionString_When_Present()
    {
        // arrange
        _azureConfigMock.Reset();
        _azureConfig = TestData.AzureConfig(useRootNamespace:false);
        _azureConfigMock.Setup(ac => ac.Value).Returns(_azureConfig);
        
        var topicConfig = _azureConfig.ServiceBus!.Topics!.First();

        // act
        var result = _serviceBusFactory.GetFullyQualifiedAsbNamespace(topicConfig);

        // assert
        Assert.NotNull(result);
        Assert.Equal( $"{topicConfig.Namespace}.servicebus.windows.net", result);
    }
    
    [Fact]
    public void Should_Use_Root_ConnectionString_When_Topic_ConnectionString_Not_Present()
    {
        // arrange
        var topicConfig = _azureConfig.ServiceBus!.Topics!.First();

        // act
        var result = _serviceBusFactory.GetFullyQualifiedAsbNamespace(topicConfig);

        // assert
        Assert.NotNull(result);
        Assert.Equal($"{_azureConfig.ServiceBus.Namespace}.servicebus.windows.net", result);
    }
}