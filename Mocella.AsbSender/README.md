# Mocella - ASB Sender

Azure Service Bus (ASB) Sender is a .NET 8 console application designed for sending messages to an Azure Service Bus Topic.

## Configuration

Utilize .NET Secrets to configure the sensitive settings not found in the committed appsettings.json file.  
We are using ManagedIdentity to authenticate with Azure Service Bus in the deployed environment, and for local, we can utilize
`az login` to authenticate with Azure and then run the application.  

If you are opting to use a connection string, you can add the connection string similar to how Namespace is configured - allowing for variations between Topics that may live in different ASB Namespaces or fallback to the common, root ConnectionString.

### Example configuration for applications using Managed Identity:
```
{
  "ConnectionStrings": {
    "MocellaDatabase": "data source=.;database=Sample;Integrated Security=True;TrustServerCertificate=Yes;MultipleActiveResultSets=true;"
  },
  "Azure": {
    "ServiceBus": {
      "Namespace": "sbx-scus-mocella-sb-zs6fwwgqywi7a",
      "Topics": [
        {
          "TopicName": "customer-events",
          "SubscriptionName": "customer-events-portal",
          "ResponseTopicName": "customer-events-rsp",
          "Namespace": "",
          "RetryPolicy": {
            "MaxRetries": 5,
            "DelayInMinutes": 1
          }
        },
        {
          "TopicName": "venue-events",
          "SubscriptionName": "venue-events-portal",
          "ResponseTopicName": "venue-events-rsp",
          "Namespace": "",
          "RetryPolicy": {
            "MaxRetries": 5,
            "DelayInMinutes": 1
          }
        }
      ]
    }
  }
}
```

