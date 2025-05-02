using AgenticApplication.Services;

public class LangFusePromptProvider : IPromptProvider
{
    public Task<string> GetPromptAsync(string promptName)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddPromptAsync(string promptName, string promptContent)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdatePromptAsync(string promptName, string promptContent)
    {
        throw new NotImplementedException();
    }
}