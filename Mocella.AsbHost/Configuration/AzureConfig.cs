namespace Mocella.AsbHost.Configuration;

public class AzureConfig
{
    public ServiceBusConfig? ServiceBus { get; set; }
}

public class ServiceBusConfig
{
    public string? Namespace { get; set; }
    public TopicConfig[]? Topics { get; set; }
}

public class TopicConfig
{
    public RetryPolicyConfig? RetryPolicy { get; set; }
    public string? TopicName { get; set; }
    public string? SubscriptionName { get; set; }
    public string? ResponseTopicName { get; set; }
    public string? Namespace { get; set; }
}

public class RetryPolicyConfig
{
    public int MaxRetries { get; set; }
    public int DelayInMinutes { get; set; }
}


