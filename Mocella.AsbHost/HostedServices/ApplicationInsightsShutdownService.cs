using Microsoft.ApplicationInsights;

namespace Mocella.AsbHost.HostedServices;

public class ApplicationInsightsShutdownService: IHostedService
{
    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<ApplicationInsightsShutdownService> _logger;

    public ApplicationInsightsShutdownService(TelemetryClient telemetryClient, 
        ILogger<ApplicationInsightsShutdownService> logger)
    {
        _telemetryClient = telemetryClient;
        _logger = logger;   
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var successfullyFlushed = await this._telemetryClient.FlushAsync(CancellationToken.None);
        if (!successfullyFlushed)
        {
            _logger.LogError("Failed to flush Application Insights telemetry");
        }
    }
}