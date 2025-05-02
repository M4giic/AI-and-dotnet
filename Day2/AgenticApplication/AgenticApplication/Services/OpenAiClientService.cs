using System.Text.Json;
using AgenticApplication.Data.Settings;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace AgenticApplication.Services;

public interface IOpenAiClientService
{
    Task<string> GetCompletionAsync(string promptName, object promptData = null);
    Task<T> GetStructuredCompletionAsync<T>(string promptName, object promptData = null) where T : class;

    Task<string> GetChatCompletionAsync(string promptName, object promptData = null,
        List<ChatMessage> previousMessages = null);

    Task<string> GetUserAndSystemCompletionAsync(string systemPromptName, string userPrompt,
        object systemPromptData = null);
}

public class OpenAiClientService : IOpenAiClientService
{
    private readonly IPromptProvider _promptProvider;
    private readonly ChatClient _chatClient;
    private readonly OpenAiSettings _settings;
    private readonly ILogger<OpenAiClientService> _logger;

    public OpenAiClientService(
        IPromptProvider promptProvider,
        IOptions<OpenAiSettings> settings,
        ILogger<OpenAiClientService> logger)
    {
        _promptProvider = promptProvider;
        _chatClient = new(model: "gpt-4o", apiKey: _settings.ApiKey);
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string> GetCompletionAsync(string promptName, object promptData = null)
    {
        try
        {
            // Get the prompt template from the provider
            var promptTemplate = await _promptProvider.GetPromptAsync(promptName);
            if (string.IsNullOrEmpty(promptTemplate))
            {
                throw new ArgumentException($"Prompt '{promptName}' not found");
            }

            // Format the prompt with the provided data
            var formattedPrompt = FormatPrompt(promptTemplate, promptData);

            // Create the completion with the official client
            ChatCompletion completion = await _chatClient.CompleteChatAsync(formattedPrompt);

            return completion.Content.FirstOrDefault()?.Text ?? string.Empty;
          
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting completion for prompt '{PromptName}'", promptName);
            throw;
        }
    }

    public async Task<T> GetStructuredCompletionAsync<T>(string promptName, object promptData = null) where T : class
    {
        try
        {
            // Get the prompt template from the provider
            var promptTemplate = await _promptProvider.GetPromptAsync(promptName);
            if (string.IsNullOrEmpty(promptTemplate))
            {
                throw new ArgumentException($"Prompt '{promptName}' not found");
            }

            // Format the prompt with the provided data
            var formattedPrompt = FormatPrompt(promptTemplate, promptData);

            // Add instructions for structured output
            var structuredPrompt = $"{formattedPrompt}\n\nProvide your response as a valid JSON object that can be parsed into the following C# class:\n{typeof(T).Name}";

            // Create the chat messages
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a helpful assistant that provides responses in structured JSON format."),
                new UserChatMessage( structuredPrompt)
            };

            ChatCompletion completion = await _chatClient.CompleteChatAsync(
                messages,
                new ChatCompletionOptions()
                {
                    Temperature = _settings.Temperature,
                    MaxOutputTokenCount = _settings.MaxTokens
                });

            var content = completion.Content.FirstOrDefault()?.Text ?? string.Empty;
            
            // Extract JSON from the content if it's wrapped in markdown code blocks
            var jsonMatch = System.Text.RegularExpressions.Regex.Match(content, @"\{[\s\S]*\}");
            if (jsonMatch.Success)
            {
                content = jsonMatch.Value;
            }
            
            try
            {
                // Deserialize the JSON response to the requested type
                var result = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                return result;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize OpenAI response to type {Type}: {Content}", typeof(T).Name, content);
                throw new Exception($"Failed to parse structured response: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting structured completion for prompt '{PromptName}'", promptName);
            throw;
        }
    }
    
     public async Task<string> GetUserAndSystemCompletionAsync(string systemPromptName, string userPrompt, object systemPromptData = null)
    {
        try
        {
            // Get the prompt template from the provider
            var promptTemplate = await _promptProvider.GetPromptAsync(systemPromptName);
            if (string.IsNullOrEmpty(promptTemplate))
            {
                throw new ArgumentException($"Prompt '{systemPromptName}' not found");
            }

            // Format the prompt with the provided data
            var formattedPrompt = FormatPrompt(promptTemplate, systemPromptData);

            // Add instructions for structured output
            // Create the chat messages
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(formattedPrompt),
                new UserChatMessage( userPrompt )
            };

            ChatCompletion completion = await _chatClient.CompleteChatAsync(
                messages,
                new ChatCompletionOptions()
                {
                    Temperature = _settings.Temperature,
                    MaxOutputTokenCount = _settings.MaxTokens
                });

            var content = completion.Content.FirstOrDefault()?.Text ?? string.Empty;

            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting structured completion for prompt '{PromptName}'", systemPromptName);
            throw;
        }
    }

    public async Task<string> GetChatCompletionAsync(string promptName, object promptData = null, List<ChatMessage> previousMessages = null)
    {
        try
        {
            // Get the prompt template from the provider
            var systemPromptTemplate = await _promptProvider.GetPromptAsync(promptName);
            if (string.IsNullOrEmpty(systemPromptTemplate))
            {
                throw new ArgumentException($"Prompt '{promptName}' not found");
            }

            // Format the system prompt with the provided data
            var formattedSystemPrompt = FormatPrompt(systemPromptTemplate, promptData);

            // Create the messages list starting with the system message
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage( formattedSystemPrompt)
            };

            // Add previous messages if provided
            

            // Create completion options
            var options = new ChatCompletionOptions
            {
                Temperature = _settings.Temperature
                // MaxTokens not directly available
            };

            // Create the completion with the official client
            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);

            return completion.Content?.FirstOrDefault()?.Text ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting chat completion for prompt '{PromptName}'", promptName);
            throw;
        }
    }
     
    public async Task<string> ContinueCompletionAsync(ChatMessage newMessage, List<ChatMessage> previousMessages)
    {
        try
        {
            List<ChatMessage> messages = previousMessages;
            messages.Add(newMessage);
            // Create completion options
            var options = new ChatCompletionOptions
            {
                Temperature = _settings.Temperature
                // MaxTokens not directly available
            };

            // Create the completion with the official client
            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);

            return completion.Content?.FirstOrDefault()?.Text ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting chat completion for prompt '{PromptName}'", newMessage);
            throw;
        }
    }
    /*
    replaces 
    
        Write a {{tone}} email to {{recipient}} about {{subject}}.
        
    After being provided with the following data:
    
        new { 
            tone = "professional", 
            recipient = "John", 
            subject = "upcoming meeting" 
        }
          
    to  
        Write a professional email to John about upcoming meeting.
    */
    
    private string FormatPrompt(string promptTemplate, object data)
    {
        if (data == null)
        {
            return promptTemplate;
        }

        var formattedPrompt = promptTemplate;

        // Get all properties from the data object
        var properties = data.GetType().GetProperties();
        foreach (var prop in properties)
        {
            var value = prop.GetValue(data)?.ToString() ?? string.Empty;
            formattedPrompt = formattedPrompt.Replace($"{{{{{prop.Name}}}}}", value);
        }

        return formattedPrompt;
    }
}