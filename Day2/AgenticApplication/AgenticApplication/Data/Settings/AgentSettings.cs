namespace AgenticApplication.Data.Settings;

public class AgentSettings
{
    public int MaxTokens { get; set; } = 1000;
    public string PromptSource { get; set; } = "Local";
}