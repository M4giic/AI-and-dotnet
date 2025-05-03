namespace AgenticApplication.Services;

public class PromptProviderSettings
{
    public string PromptsDirectory { get; set; } = "Prompts";
    public string PromptFileExtension { get; set; } = ".prompt";
    public bool UseCache { get; set; } = true;
}