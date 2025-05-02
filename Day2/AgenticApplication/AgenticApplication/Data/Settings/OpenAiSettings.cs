namespace AgenticApplication.Data.Settings;
public class OpenAiSettings
{
    public string DefaultModelId { get; set; } = "gpt-4";
    public int MaxTokens { get; set; } = 1000;
    public float Temperature { get; set; } = 0.7f;
    public string ApiKey { get; set; }
}