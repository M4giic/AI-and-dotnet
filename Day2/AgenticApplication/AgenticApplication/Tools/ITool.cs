using AgenticApplication.Data.Models;

namespace AgenticApplication.Tools;

public interface ITool
{
    string ToolName { get; }
    string GetDescription(); 
    Task<ToolResult> ExecuteAsync(ToolTask task);  
}