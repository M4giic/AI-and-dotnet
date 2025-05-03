using AgenticApplication.Data.Models;

namespace AgenticApplication.Services;

public interface IToolingService
{
    Task<ToolResult> ExecuteTaskAsync(ToolTask task, Dictionary<string, ToolResult> previousResults);
    string GetAllToolsDescription();
}