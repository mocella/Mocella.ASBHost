using Azure.Messaging.ServiceBus;

namespace Mocella.AsbHost.ServiceBus;

public class MessageReceiver : IMessageReceiver
{
    private readonly ServiceBusProcessor _processor;
    
    public MessageReceiver(ServiceBusProcessor processor)
    {
        _processor = processor;
    }

    public Task<Task> Receive(Func<ProcessMessageEventArgs, Task> messageHandler,
        Func<ProcessErrorEventArgs, Task> exceptionHandler, CancellationToken cancellationToken = default)
    {
        // add handler to process messages
        _processor.ProcessMessageAsync += messageHandler;

        // add handler to process any errors
        _processor.ProcessErrorAsync += exceptionHandler;
        
        return Task.FromResult(Task.CompletedTask);
    }

    public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
    {
        await _processor.StartProcessingAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task StopProcessingAsync(CancellationToken cancellationToken = default)
    {
        return _processor.StopProcessingAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await _processor.DisposeAsync().ConfigureAwait(false);
    }
}