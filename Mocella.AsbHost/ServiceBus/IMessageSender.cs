using Azure.Messaging.ServiceBus;

namespace Mocella.AsbHost.ServiceBus;

public interface IMessageSender : IAsyncDisposable
{
    Task SendMessageAsync(ServiceBusMessage message, 
        CancellationToken cancellationToken = default(CancellationToken));

    Task ScheduleMessageAsync(ServiceBusMessage message, 
        DateTimeOffset scheduledEnqueueTime,
        CancellationToken cancellationToken = default(CancellationToken));
}