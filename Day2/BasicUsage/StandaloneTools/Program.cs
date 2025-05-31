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
var apiKey = configuration["OpenAI:ApiKey"];

// Initialize email settings
var emailSettings = new EmailSettings
{
    SenderName = "Agent CLI",
    SenderEmail = "agent@example.com",
    Username = "jendraas@gmail.com", // Replace with your actual email
    Password = "hidden-password", // Replace with your actual app password
    ApiKey = apiKey
};

// Check if API key is available
if (string.IsNullOrEmpty(emailSettings.ApiKey))
{
    Console.WriteLine("Error: OpenAI API key not found in user secrets.");
    Console.WriteLine("Please set it with: dotnet user-secrets set \"OpenAI:ApiKey\" \"your-api-key-here\"");
    return;
}

// Initialize the agent processor
var agentProcessor = new AgentProcessor(emailSettings);

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
        // Step 1: Planning - Analyze user input to identify tasks
        var tasks = await agentProcessor.PlanAsync(userInput);
        
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks identified from your request. Please try again with a clearer request.");
            continue;
        }
        
        // Process each identified task
        foreach (var task in tasks)
        {
            Console.WriteLine($"Processing task: {task.ToolName} - {task.Instruction}");
            
            // Step 2: Action - Prepare structured action sequence
            var actionSequence = await agentProcessor.PrepareActionSequenceAsync(task);
            
            // Step 3: Tool Use - Execute each action in the sequence
            foreach (var action in actionSequence.Actions)
            {
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
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}

Console.WriteLine("Thank you for using the Agent CLI Tool. Goodbye!");