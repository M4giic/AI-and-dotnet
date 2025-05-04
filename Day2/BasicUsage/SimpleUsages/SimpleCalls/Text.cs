using OpenAI.Chat;

namespace SimpleUsages.SimpleCalls;

public class Text
{
    public void TryToGuess(string apiKey)
    {
        ChatClient chatClient = new ChatClient(model: "gpt-4.1", apiKey: apiKey);
        var systemPrompt = GetSystemPromptGuess(); 
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
        };
        
        while (true)
        {
            Console.WriteLine("You are tasked with guessing a word. " +
                              "The assistant will give you hints and you can ask questions. " +
                              "If you guess the word, the assistant will say 'congrats!'.");
            Console.Write("\nYour guess: ");
            var prompt = Console.ReadLine();

            if (string.IsNullOrEmpty(prompt) || prompt.ToLower() == "exit")
            {
                return;
            }

            try
            {
                // Call the OpenAI API
               messages.Add(new UserChatMessage( prompt ) );

                ChatCompletion completion = chatClient.CompleteChat(messages);

                var content = completion.Content.FirstOrDefault()?.Text ?? string.Empty;

                if (!String.IsNullOrEmpty(content))
                {
                    Console.WriteLine("\nGPT's response:");
                    Console.WriteLine(content);
                    messages.Add(new AssistantChatMessage(completion));
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
    }

    public void EndlessConversation(string apiKey)
    {
        ChatClient chatClient = new ChatClient(model: "gpt-4.1", apiKey: apiKey);

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
                var systemPrompt = GetSystemPromptConversation(); 
                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage(systemPrompt),
                    new UserChatMessage( prompt ),
                };

                ChatCompletion completion = chatClient.CompleteChat(messages);

                var content = completion.Content.FirstOrDefault()?.Text ?? string.Empty;

                if (!String.IsNullOrEmpty(content))
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
    }
    
    private string GetSystemPromptGuess()
    {
        return "We are playing a game. You are meant  to think of a word and I will try to guess it. " +
               "I will say a word and you have to tell me how relevant is my word to yours. " + 
               "If I guess the word, you will say 'congrats!' " +
               "Let's start! Think of a word and give me the first hint.";
    }
    
    private string GetSystemPromptConversation()
    {
        //write a converstation system prompt
        return "I will send you a message and you will respond to it like a bond villain";  
    }
}