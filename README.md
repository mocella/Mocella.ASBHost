# azure-service-bus-host
Example Azure ServiceBus (ASB) Sender/Receiver Processes in .NET8 

## AsbHost
.NET8 ASP.Net project, used primarily for hosting HostedServices which are acting as ASB Message Receivers.  Receiver processes are driven by a common TopicConfig section that allows the ServiceBusFactory to create the appropriately configured Message Receiver, Responder (when applicable) as well as retry behaviors.

## AsbSender
.NET8 console app used to send messages to ASB Topics.  See [AsbSender README.md](/Mocella.AsbSender/README.md) for further details/configuration examples.