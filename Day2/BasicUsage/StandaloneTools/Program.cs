using Microsoft.Extensions.Configuration;
using Tools.Agent;
using Tools.Models;

// Set up configuration with user secrets
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

Console.WriteLine("Welcome to the Agent CLI Tool");
Console.WriteLine("------------------------------");

// Get API key from user secrets
var apiKeyOpenAi = configuration["OpenAI:ApiKey"] ?? string.Empty;
var apiKeyMail = configuration["EmailSettings:ApiKey"] ?? string.Empty;
var secretKeyMail = configuration["EmailSettings:SecretKey"] ?? string.Empty;

// Initialize agent settings
var agentSettings = new AgentSettings
{
    OpenAIApiKey = apiKeyOpenAi
};

// Initialize email settings
var emailSettings = new EmailSettings
{
    SenderName = "Agent CLI",
    SenderEmail = "jendraas@gmail.com",
    Username = "jendraas@gmail.com", // Replace with your actual email
    Password = "hidden-password", // Replace with your actual app password
    ApiKey = apiKeyMail,
    SecretKey = secretKeyMail,
};

// Initialize the agent processor with both settings
var agentProcessor = new AgentProcessor(emailSettings, agentSettings);

while (true)
{
    Console.WriteLine("\nEnter your request (or 'exit' to quit):");
    Console.Write("> ");

    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput))
    {
        continue;
    }

    if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    try
    {
        var actionSequence = await agentProcessor.CreateActionSequenceAsync(userInput);

        if (actionSequence == null || actionSequence.Actions == null || actionSequence.Actions.Count == 0)
        {
            Console.WriteLine("No actions could be created from your request. Please try again with a clearer request.");
            continue;
        }

        // Execute each action in the sequence
        foreach (var action in actionSequence.Actions)
        {
            Console.WriteLine($"Processing action: {action.ToolName} - {action.Instruction}");

            var result = await agentProcessor.ExecuteToolAsync(action);

            if (result.Success)
            {
                Console.WriteLine($"✅ Success: {result.Message}");
            }
            else
            {
                Console.WriteLine($"❌ Error: {result.Message}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}

Console.WriteLine("Thank you for using the Agent CLI Tool. Goodbye!");
