using Mocella.AsbHost.ServiceBus;

namespace Mocella.AsbHost.HostedServices;

public class PortalCustomerHostedService : BaseHostedService<PortalCustomerHostedService>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    protected virtual Task Shutdown() => Task.CompletedTask;

    public PortalCustomerHostedService(ILogger<PortalCustomerHostedService> logger,
        IServiceScopeFactory serviceScopeFactory) : base(logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation($"Starting {nameof(PortalCustomerHostedService)} service ...");
        _ = DoWork(cancellationToken); // has to be this way or the first HostedService blocks the rest
        return Task.FromResult(Task.CompletedTask);
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        using (IServiceScope scope = _serviceScopeFactory.CreateAsyncScope())
        {
            var portalCustomerEventService = scope.ServiceProvider.GetRequiredService<CustomerEventService>();
            Logger.LogInformation($"{nameof(PortalCustomerHostedService)} service started");    
            await portalCustomerEventService.Processor!.StartProcessingAsync(cancellationToken).ConfigureAwait(false);
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation($"Stopping {nameof(PortalCustomerHostedService)} service ...");
        await Shutdown();
        Logger.LogInformation($"{nameof(PortalCustomerHostedService)} service stopped");
    }
}