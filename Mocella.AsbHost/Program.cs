using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Mocella.AsbHost.Configuration;
using Mocella.AsbHost.HostedServices;
using Mocella.AsbHost.ServiceBus;
using Mocella.AsbHost.ServiceBus.Senders;
using Mocella.AsbHost.Services;
using Mocella.AsbHost.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

builder.Services.AddControllers();

builder.Services.AddApplicationInsightsTelemetry();
builder.Logging.AddApplicationInsights();

builder.Services.AddSingleton<IServiceBusFactory, ServiceBusFactory>();
builder.Services.AddSingleton<ILoggerFactory, LoggerFactory>();


#pragma warning disable ASP0000
var serviceProvider = builder.Services.BuildServiceProvider();
#pragma warning restore ASP0000
var logger = serviceProvider.GetService<ILogger<IMocellaService>>();
builder.Services.AddSingleton(typeof(ILogger), logger!);


builder.Services.Configure<AzureConfig>(builder.Configuration.GetSection("Azure"));
builder.Services.AddValidatorsFromAssemblyContaining<MocellaCustomerRequestValidator>();

builder.AddServiceBusServiceClasses<Program>(ServiceLifetime.Transient); 
builder.AddMocellaServiceClasses<MocellaCustomerService>(ServiceLifetime.Transient);
builder.AddServiceBusSenders<CustomerEventSender>();

builder
    .Services
    .Configure<HostOptions>(options =>
    {
        // Extend the host shutdown to ensure that the processor infrastructure has
        // time to cleanly shut down.
        //
        // Applications which do not support cancellation in their processing handler
        // advised to extend this to ensure that adequate time is allowed to complete
        // processing for an event that in-flight when stopping was requested.
        options.ShutdownTimeout = TimeSpan.FromSeconds(30);
    });

builder.Services.AddHostedService<PortalCustomerHostedService>();
builder.Services.AddHostedService<ApplicationInsightsShutdownService>();

var app = builder.Build();

if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<string>("EnableSwagger") == "true")
{
    // this app isn't intended to be an API, but does support that so wiring up Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowLocalHost");
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapControllers();

app.Run();