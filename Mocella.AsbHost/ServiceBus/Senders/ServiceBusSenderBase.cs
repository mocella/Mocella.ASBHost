using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using Microsoft.ApplicationInsights;
using Mocella.AsbHost.Configuration;

namespace Mocella.AsbHost.ServiceBus.Senders;

public abstract class ServiceBusSenderBase<T, TM> : IAsyncDisposable, IServiceBusSenderBase
{
    private readonly IMessageSender _sender;

    protected readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true,
        IncludeFields = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull //saves significantly on payload size 
    };

    protected readonly ILogger<T> Logger;
    protected readonly TelemetryClient TelemetryClient;
    private readonly IServiceBusFactory _serviceBusFactory;

    protected ServiceBusSenderBase(TopicConfig config,
        IServiceBusFactory serviceBusFactory,
        ILogger<T> logger,
        TelemetryClient telemetryClient)
    {
        _serviceBusFactory = serviceBusFactory;
        Logger = logger;
        TelemetryClient = telemetryClient;
        _sender = _serviceBusFactory.GetTopicSender(config!.TopicName!);
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _serviceBusFactory.DisposeAsync();
    }

    public abstract Task CreateAndSendMessage(TM eventDetails,
        RequestAction requestAction,
        DateTime sourceLastUpdatedDateUtc);

    protected void SendMessageAsync(string messageBody)
    {
        TelemetryClient.TrackTrace("ServiceBusSenderBase - SendMessageAsync");
        // NOTE: not awaiting this task as we don't want to block the calling thread
        _sender.SendMessageAsync(new ServiceBusMessage(messageBody)
        {
            CorrelationId = Trace.CorrelationManager.ActivityId.ToString(),
            ApplicationProperties =
            {
                { AsbHostConstants.SourceSystemProperty, AsbHostConstants.SourceSystemPortal }
            }
        });
        TelemetryClient.TrackTrace("ServiceBusSenderBase - SendMessageAsync - Sent");
    }
}