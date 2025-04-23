using Mocella.AsbHost.Configuration;

namespace Mocella.AsbHost.ServiceBus;

public interface IServiceBusFactory : IAsyncDisposable
{
    IMessageSender GetTopicSender(string topicName);
    IMessageSender GetTopicResponseSender(string topicName);
    IMessageSender GetTopicRetrySender(string topicName);
    IMessageReceiver GetTopicReceiver(string topicName);
    string? GetFullyQualifiedAsbNamespace(TopicConfig? topicConfig);
}