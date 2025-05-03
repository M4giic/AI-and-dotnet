using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;

Console.WriteLine("Hello, World!");

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
    
    
var serviceProvider = new ServiceCollection()
    .AddSingleton<IConfiguration>(configuration)
    .AddSingleton<ChatClient>(_ => 
    {
        var apiKey = configuration["OpenAI:ApiKey"];
        return new(model: "gpt-4.1", apiKey);
    })
    .BuildServiceProvider();

// Get the OpenAI service
var chatClient = serviceProvider.GetRequiredService<ChatClient>();

Console.WriteLine("Welcome to the OpenAI Console App!");
Console.WriteLine("Type a prompt and press Enter to get a response from GPT. Type 'exit' to quit.");

while (true)
{
    Console.Write("\nYour prompt: ");
    var prompt = Console.ReadLine();

    if (string.IsNullOrEmpty(prompt) || prompt.ToLower() == "exit")
    {
        return;
    }

    try
    {
        // Call the OpenAI API
        var systemPrompt = GetSystemPrompt(); 
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage( prompt )
        };

        ChatCompletion completion = await chatClient.CompleteChatAsync(messages);

        var content = completion.Content.FirstOrDefault()?.Text ?? string.Empty;

        if (String.IsNullOrEmpty(content))
        {
            Console.WriteLine("\nGPT's response:");
            Console.WriteLine(content);
        }
        else
        {
            Console.WriteLine("GPT Failed.");
            Console.WriteLine(completion);
            return;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex.Message}");
    }
}

string GetSystemPrompt()
{
    return "You are a helpful assistant. Respond to the user's message.";
}