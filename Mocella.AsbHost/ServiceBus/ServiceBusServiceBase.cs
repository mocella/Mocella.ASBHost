using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Mocella.AsbHost.Configuration;

namespace Mocella.AsbHost.ServiceBus;

public abstract class ServiceBusServiceBase<T> : IAsyncDisposable, IServiceBusServiceBase
{
    public readonly IMessageReceiver? Processor;
    protected IMessageSender RetrySender { get; set; }
    protected IMessageSender? ResponseSender { get; set; }
    protected readonly ILogger<T> Logger;
    protected readonly TelemetryClient TelemetryClient;

    public readonly JsonSerializerOptions JsonSerializerOptions = new()
    { 
        WriteIndented = true, 
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true,
        IncludeFields = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull //saves significantly on payload size 
    };

    private readonly RetryPolicyConfig? _retryConfig;
    private readonly IServiceBusFactory _serviceBusFactory;
    
    protected ServiceBusServiceBase(TopicConfig config, 
        IServiceBusFactory serviceBusFactory, 
        ILogger<T> logger,
        TelemetryClient telemetryClient)
    {
        Logger = logger;
        TelemetryClient = telemetryClient;
        _serviceBusFactory = serviceBusFactory;
        Processor = _serviceBusFactory.GetTopicReceiver(config!.TopicName!);
        RetrySender = _serviceBusFactory.GetTopicRetrySender(config!.TopicName!);
        
        if (!string.IsNullOrWhiteSpace(config.ResponseTopicName))
        {
            ResponseSender = _serviceBusFactory.GetTopicResponseSender(config.TopicName!);
        }
        
        _retryConfig = config.RetryPolicy!;

        Processor?.Receive(MessageHandler<T>, ErrorHandler<T>);
    }

    protected virtual async Task MessageHandler<T2>(ProcessMessageEventArgs args)
    {
        Trace.CorrelationManager.ActivityId = string.IsNullOrWhiteSpace(args.Message.CorrelationId)
            ? Guid.NewGuid()
            : Guid.Parse(args.Message.CorrelationId);

        var activity = new Activity("ServiceBusProcessor.ProcessMessage");
        activity.SetParentId(Trace.CorrelationManager.ActivityId.ToString());

        using var operation = TelemetryClient.StartOperation<RequestTelemetry>(activity);
        try
        {
            var responseMessage = await ProcessMessage(args);

            var messageBody = JsonSerializer.Serialize(responseMessage, JsonSerializerOptions);
            await SendResponseMessage(new ServiceBusMessage(messageBody));

            await args.CompleteMessageAsync(args.Message);
            TelemetryClient.TrackTrace("Done");
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"Error processing message. CorrelationId: {args.Message.CorrelationId}");
            TelemetryClient.TrackException(e);
            operation.Telemetry.Success = false;
            ScheduleRetry(args);
        }
        finally
        {
            TelemetryClient.StopOperation(operation);
        }
    }

    public abstract Task<object> ProcessMessage(ProcessMessageEventArgs args);
    
    protected virtual Task ErrorHandler<T2>(ProcessErrorEventArgs args)
    {
        Logger.LogError(args.Exception, $"Error processing message");
        return Task.CompletedTask;
    }
    
    protected Task SendResponseMessage(ServiceBusMessage message)
    {
        if(ResponseSender != null)
        {
            if (string.IsNullOrWhiteSpace(message.CorrelationId))
            {
                message.CorrelationId = Trace.CorrelationManager.ActivityId.ToString();
            }
            return ResponseSender!.SendMessageAsync(message);
        }
        
        return Task.CompletedTask;
    }
    
    protected virtual void ScheduleRetry(ProcessMessageEventArgs args)
    {
        Logger.LogInformation($"Determining whether to retry message.  CorrelationId: {args.Message.CorrelationId}");
        TelemetryClient.TrackTrace("ScheduleRetry");
        
        args.Message.ApplicationProperties.TryGetValue(AsbHostConstants.RetryAttempts, out var attemptCount);
        var attempts = attemptCount as int? ?? 0;
        if (attempts > Math.Max(_retryConfig!.MaxRetries - 1, 1))
        {
            TelemetryClient.TrackTrace("RetryAttemptsExhausted");
            Logger.LogInformation($"Maximum retry limit exceeded. CorrelationId: {args.Message.CorrelationId}");
            return;
        }
        var delayMinutes = CalculateDelayMinutes(attempts);
        
        var delayMessage = new ServiceBusMessage(args.Message.Body);
        foreach (var messageApplicationProperty in args.Message.ApplicationProperties)
        {
            delayMessage.ApplicationProperties.Add(messageApplicationProperty);
        }
        delayMessage.CorrelationId = args.Message.CorrelationId;
        delayMessage.ApplicationProperties[AsbHostConstants.RetryAttempts] = ++attempts;

        RetrySender.ScheduleMessageAsync(delayMessage, delayMinutes);
        TelemetryClient.TrackTrace("RetryScheduled");
    }

    public async ValueTask DisposeAsync()
    {
        if(Processor != null)
        {
            await Processor.StopProcessingAsync().ConfigureAwait(false);
            await Processor.DisposeAsync();
        }
        
        await RetrySender.DisposeAsync();
        
        if(ResponseSender != null)
            await ResponseSender.DisposeAsync();
        
        await _serviceBusFactory.DisposeAsync();
    }
    
    private DateTime CalculateDelayMinutes(int attempts)
    {
        // not exactly linear, but close enough (5 attempts gives delays of: 1,1,2,3,4)
        if (attempts == 0) return DateTime.Now.AddMinutes(_retryConfig!.DelayInMinutes);

        var delayMinutes = _retryConfig!.DelayInMinutes * attempts;
        var retryTime = DateTime.Now.AddMinutes(delayMinutes);
        return retryTime;
    }
}