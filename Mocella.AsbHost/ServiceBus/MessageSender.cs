using Azure.Messaging.ServiceBus;

namespace Mocella.AsbHost.ServiceBus;

public class MessageSender : IMessageSender
{
    private readonly ServiceBusSender _sender;

    public MessageSender(ServiceBusSender sender)
    {
        _sender = sender;
    }

    public async Task SendMessageAsync(ServiceBusMessage message, 
        CancellationToken cancellationToken = default(CancellationToken))
    {
        await _sender.SendMessageAsync(message, cancellationToken);
    }

    public async Task ScheduleMessageAsync(ServiceBusMessage message, 
        DateTimeOffset scheduledEnqueueTime,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        await _sender.ScheduleMessageAsync(message, scheduledEnqueueTime, cancellationToken);
    }
    
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await _sender.DisposeAsync().ConfigureAwait(false);
    }
}