using System.Collections.Concurrent;
using AgenticApplication.Data.Prompts;
using Microsoft.Extensions.Options;

namespace AgenticApplication.Services;

public class FileSystemPromptProvider : IPromptProvider
{
    private readonly PromptProviderSettings _settings;
    private readonly ILogger<FileSystemPromptProvider> _logger;
    private readonly ConcurrentDictionary<string, string> _promptCache = new ConcurrentDictionary<string, string>();

    public FileSystemPromptProvider(
        IOptions<PromptProviderSettings> settings,
        ILogger<FileSystemPromptProvider> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        // Create the prompts directory if it doesn't exist
        if (!Directory.Exists(_settings.PromptsDirectory))
        {
            Directory.CreateDirectory(_settings.PromptsDirectory);
        }
    }
    
    public Task<string> GetPromptAsync(string promptName)
    {
        try
        {
            if (_promptCache.ContainsKey(promptName))
            {
                return Task.FromResult(_promptCache[promptName]);
            }
            // Search for types in the current assembly that implement PromptBase
            var assembly = typeof(PromptBase).Assembly;
            var promptType = assembly.GetTypes()
                .FirstOrDefault(t => typeof(PromptBase).IsAssignableFrom(t) && t.Name == promptName);

            if (promptType == null)
            {
                _logger.LogWarning("Prompt type not found: {PromptName}", promptName);
                return null;
            }

            // Create an instance of the prompt type
            var promptInstance = Activator.CreateInstance(promptType);
            if (promptInstance == null)
            {
                _logger.LogError("Failed to create instance of prompt: {PromptName}", promptName);
                return null;
            }

            var property = promptType.GetProperty("Content");
            var propertyValue = (string)property.GetValue(promptInstance);

            _promptCache[promptName] = propertyValue;
            return Task.FromResult(propertyValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting prompt '{PromptName}'", promptName);
            return null;
        }
    }

}

