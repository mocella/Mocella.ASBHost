using Mocella.AsbHost.RequestObjects;

namespace Mocella.AsbHost.Services;

public class MocellaCustomerService : IMocellaCustomerService
{
    private readonly ILogger<MocellaCustomerService> _logger;

    public MocellaCustomerService(ILogger<MocellaCustomerService> logger)
    {
        // TODO: TelemetryClient vs Logger
        _logger = logger;
    }

    public CustomerEvent Add(CustomerEvent addEvent)
    {
        _logger.LogInformation($"Processing AddEvent for Customer: {addEvent.CustomerName}");
        return addEvent;
    }

    public CustomerEvent Update(CustomerEvent updateEvent)
    {
        _logger.LogInformation($"Processing UpdateEvent for Customer: {updateEvent.CustomerName}");
        return updateEvent;
    }

    public CustomerEvent Delete(CustomerEvent deleteEvent)
    {
        _logger.LogInformation($"Processing DeleteEvent for Customer: {deleteEvent.CustomerName}");
        return deleteEvent;
    }
}