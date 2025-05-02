namespace AgenticApplication.Services;

public interface IPromptProvider
{
    Task<string> GetPromptAsync(string promptName);
}