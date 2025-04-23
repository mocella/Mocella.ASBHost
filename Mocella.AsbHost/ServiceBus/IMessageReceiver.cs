using Azure.Messaging.ServiceBus;

namespace Mocella.AsbHost.ServiceBus;

public interface IMessageReceiver: IAsyncDisposable
{
    Task<Task> Receive(Func<ProcessMessageEventArgs, Task> messageHandler,
        Func<ProcessErrorEventArgs, Task> exceptionHandler,
        CancellationToken cancellationToken = default);
    
    Task StartProcessingAsync(CancellationToken cancellationToken = default);
    Task StopProcessingAsync(CancellationToken cancellationToken = default);
}