using System.Collections.Concurrent;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using Mocella.AsbHost.Configuration;

namespace Mocella.AsbHost.ServiceBus;

public class ServiceBusFactory : IServiceBusFactory
{
    private readonly ServiceBusConfig _serviceBusConfig;
    private readonly ConcurrentDictionary<string, IMessageSender> _senders;
    private readonly ConcurrentDictionary<string, IMessageReceiver> _receivers;
    private readonly ConcurrentDictionary<string, IMessageSender> _retrySenders;
    private readonly ILogger<ServiceBusFactory> _logger;

    public ServiceBusFactory(
        IOptions<AzureConfig> azureConfig,
        ILogger<ServiceBusFactory> logger)
    {
        _serviceBusConfig = azureConfig.Value.ServiceBus ??
                            throw new ArgumentException("Missing Azure::ServiceBus configuration.");
        _logger = logger ?? 
                  throw new ArgumentNullException(nameof(logger));

        _senders = new ConcurrentDictionary<string, IMessageSender>();
        _receivers = new ConcurrentDictionary<string, IMessageReceiver>();
        _retrySenders = new ConcurrentDictionary<string, IMessageSender>();
    }

    public IMessageSender GetTopicSender(string topicName)
    {
        return _senders.GetOrAdd(topicName, CreateTopicSender);
    }

    public IMessageSender GetTopicResponseSender(string topicName)
    {
        var topicConfig = GetTopicConfig(topicName);
        if (string.IsNullOrWhiteSpace(topicConfig!.ResponseTopicName))
            throw new ArgumentException($"Missing configuration of ResponseTopicName for Azure:ServiceBus:Topics[{topicName}].");

        return _senders.GetOrAdd(topicConfig.ResponseTopicName, CreateTopicResponseSender(topicConfig));
    }  

    public IMessageSender GetTopicRetrySender(string topicName)
    {
        return _retrySenders.GetOrAdd(topicName, CreateTopicRetrySender);
    }

    public IMessageReceiver GetTopicReceiver(string topicName)
    {
        return _receivers.GetOrAdd(topicName, CreateMessageReceiver(topicName));
    }
    
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        foreach (var (_, sender) in _senders)
            await sender.DisposeAsync();
        
        foreach (var (_, sender) in _retrySenders)
            await sender.DisposeAsync();

        foreach (var (_, receiver) in _receivers)
            await receiver.DisposeAsync();
    }

    private MessageReceiver CreateMessageReceiver(string topicName)
    {
        var topicConfig = GetTopicConfig(topicName);
        var client = GetServiceBusClient(topicConfig!);
        var processor = client.CreateProcessor(topicConfig!.TopicName, topicConfig.SubscriptionName);
        return new MessageReceiver(processor);
    }

    private MessageSender CreateTopicRetrySender(string topicName)
    {
        var topicConfig = GetTopicConfig(topicName);
        var client = GetServiceBusClient(topicConfig!);
        var sender = client.CreateSender(topicConfig!.TopicName);
        _logger.LogInformation($"Retry-Sender created for Topic: '{topicConfig.TopicName}'");
        return new MessageSender(sender);
    }

    private MessageSender CreateTopicSender(string topicName)
    {
        var topicConfig = GetTopicConfig(topicName);
        var client = GetServiceBusClient(topicConfig!);
        var sender = client.CreateSender(topicConfig!.TopicName);
        _logger.LogInformation($"Sender created for Topic: '{topicConfig.TopicName}'");
        return new MessageSender(sender);
    }

    private MessageSender CreateTopicResponseSender(TopicConfig topicConfig)
    {
        var client = GetServiceBusClient(topicConfig);
        var sender = client.CreateSender(topicConfig.ResponseTopicName);
        _logger.LogInformation($"Sender created for Response-Topic: '{topicConfig.ResponseTopicName}'");
        return new MessageSender(sender);
    }

    private ServiceBusClient GetServiceBusClient(TopicConfig topicConfig)
    {
        var credentials = GetAzureCredentials();
        var fullyQualifiedAsbNamespace = GetFullyQualifiedAsbNamespace(topicConfig);
        var client = new ServiceBusClient(fullyQualifiedAsbNamespace, credentials);
        return client;
    }

    public string GetFullyQualifiedAsbNamespace(TopicConfig? topicConfig)
    {
        var asbNamespace = !string.IsNullOrWhiteSpace(topicConfig!.Namespace)
            ? topicConfig.Namespace
            : _serviceBusConfig.Namespace;
        
        return $"{asbNamespace}.servicebus.windows.net";
    }

    private ChainedTokenCredential GetAzureCredentials()
    {
        return new ChainedTokenCredential(new ManagedIdentityCredential(), new AzureCliCredential());
    }

    private TopicConfig? GetTopicConfig(string topicName)
    {
        var topics = _serviceBusConfig.Topics;
        var topic = topics!.FirstOrDefault(t => t.TopicName == topicName);
        if (topic == null)
            throw new ArgumentException($"Topic {topicName} not found in configuration.");
        return topic;
    }
}