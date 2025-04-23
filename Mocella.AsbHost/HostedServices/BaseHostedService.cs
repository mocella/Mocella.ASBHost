namespace Mocella.AsbHost.HostedServices;

public abstract class BaseHostedService<T> : IHostedService
{
    protected readonly  ILogger<T> Logger;
    
    public BaseHostedService(ILogger<T> logger) {
        Logger = logger;
    }
    
    public abstract Task StartAsync(CancellationToken cancellationToken);

    public abstract Task StopAsync(CancellationToken cancellationToken);
}