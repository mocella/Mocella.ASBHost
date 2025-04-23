using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Mocella.AsbHost.Configuration;
using Mocella.AsbHost.RequestObjects;
using Mocella.AsbHost.Tests;
using Spectre.Console;

// TODO: to send more than one, the logic for Update/Delete needs to account for that and prompt in each loop pass for the Id
const int numOfMessages = 1;
var options = new JsonSerializerOptions
{
    WriteIndented = true,
    ReferenceHandler = ReferenceHandler.IgnoreCycles,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull //saves significantly on payload size 
};

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

var azureConfig = configuration.GetSection("Azure").Get<AzureConfig>();

var topicName = PromptForTopicName(azureConfig);

// configure the Service Bus client based on selected topicName
var topicConfig = azureConfig!.ServiceBus!.Topics!.First(c => c.TopicName == topicName);
var azureCredentials = new AzureCliCredential(); // note: requires `az login` prior to running this app
var fullyQualifiedNamespace = GetFullyQualifiedAsbNamespace(topicConfig, azureConfig.ServiceBus);

await using var client = new ServiceBusClient(fullyQualifiedNamespace, azureCredentials);
await using var sender = client.CreateSender(topicName);

// prompt for action/id (when appropriate)
var requestActionString = PromptForRequestAction();
Enum.TryParse(requestActionString, out RequestAction requestAction);
var id = PromptForId(requestAction);

AnsiConsole.Write(new Markup(
    $"[skyblue3]Preparing to send a batch of {numOfMessages} messages with action: '{requestAction}' to the '{topicName}' topic.[/]"));
AnsiConsole.WriteLine();

var sourceSystem = "portal";
using var messageBatch = await sender.CreateMessageBatchAsync();
for (var i = 1; i <= numOfMessages; i++)
{
    object? requestDetails;

    if (topicName == AsbHostConstants.CustomerEventsTopicName)
    {
        // TODO: for Update/Delete actions, may want to prompt for the Id in each loop pass and do a lookup in the Database to confirm existence
        var customerEvent = TestData.CustomerEvent();
        requestDetails = new MocellaCustomerRequest(customerEvent, 
            requestAction, 
            DateTime.UtcNow, 
            DateTime.UtcNow,
            sourceSystem);
    }
    else
    {
        // TODO: add support for additional TopicNames as we build out the app
        var ex = new Exception("The topic name is not supported by AsbSender.");
        AnsiConsole.WriteException(ex);
        return;
    }

    var messageBody = JsonSerializer.Serialize(requestDetails, options);
    
    if (!messageBatch.TryAddMessage(new ServiceBusMessage(messageBody)
        {
            CorrelationId = Guid.NewGuid().ToString(),
            ApplicationProperties =
            {
                { AsbHostConstants.SourceSystemProperty, sourceSystem } // TODO: change the sourcesystem to do testing of various Consumer processes
            }
        }))
    {
        // if it is too large for the batch, we need to handle that
        var ex = new Exception($"The message {i} is too large to fit in the batch.");
        AnsiConsole.WriteException(ex);
        return;
    }
}

try
{
    // Use the producer client to send the batch of messages to the Service Bus topic
    await sender.SendMessagesAsync(messageBatch);
    AnsiConsole.Write(new Markup($"[skyblue3]A batch of {numOfMessages} messages has been published to the topic.[/]"));
    AnsiConsole.WriteLine();
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
    return;
}

AnsiConsole.Write(new Markup("[green]Press any key to end the application[/]"));
AnsiConsole.WriteLine();
Console.ReadKey();
return;

Guid? PromptForId(RequestAction action)
{
    Guid? objectId = null;
    if (action != RequestAction.Add)
        objectId = AnsiConsole.Prompt(
            new TextPrompt<Guid>($"What is the Id for this [green]{action}[/]?")
                .PromptStyle("green")
                .ValidationErrorMessage("[red]That's not a valid id[/]")
                .Validate(guid =>
                {
                    var isValid = Guid.TryParse(guid.ToString(), out var _);
                    return isValid
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]That's not a valid id[/]");
                }));

    return objectId;
}

string PromptForRequestAction()
{
    var selectedAction = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("What type of [green]action[/] should be performed?")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more actions)[/]")
            .AddChoices([
                RequestAction.Add.ToString(), RequestAction.Update.ToString(), RequestAction.Delete.ToString()
            ]));
    return selectedAction;
}

string? PromptForTopicName(AzureConfig? azureConfig1)
{
#pragma warning disable CS8714 // If this is null, we need to blow up anyhow, and this is just a test project so not super concerned with ignoring errors
    var topicNames = azureConfig1!.ServiceBus!.Topics!.Select(c => c.TopicName).ToArray();
    var userSelection = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Which topic would you like to send [green]messages[/] to?")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more topics)[/]")!
            .AddChoices(topicNames));
#pragma warning restore CS8714 // If this is null, we need to blow up anyhow, and this is just a test project so not super concerned with ignoring errors
    return userSelection;
}

string GetFullyQualifiedAsbNamespace(TopicConfig? topicConfig1, ServiceBusConfig serviceBusConfig)
{
    var asbNamespace = !string.IsNullOrWhiteSpace(topicConfig1!.Namespace)
        ? topicConfig1.Namespace
        : serviceBusConfig.Namespace;
        
    return $"{asbNamespace}.servicebus.windows.net";
}