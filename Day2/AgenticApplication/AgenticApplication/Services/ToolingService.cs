using AgenticApplication.Data.Models;

namespace AgenticApplication.Services;

public class ToolingService : IToolingService
{
    private readonly IToolFactory _toolFactory;
    private readonly ILogger<ToolingService> _logger;

    public ToolingService(IToolFactory toolFactory, ILogger<ToolingService> logger)
    {
        _toolFactory = toolFactory ?? throw new ArgumentNullException(nameof(toolFactory));
        
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ToolResult> ExecuteTaskAsync(ToolTask task, Dictionary<string, ToolResult> previousResults = null)
    {
        if (task == null || string.IsNullOrEmpty(task.ToolName))
        {
            return new ToolResult
            {
                Success = false,
                Message = "Invalid task: Tool name is required"
            };
        }

        _logger.LogInformation("Executing task with tool: {ToolName}", task.ToolName);

        try
        {
            if (!_toolFactory.HasTool(task.ToolName))
            {
                return new ToolResult
                {
                    Success = false,
                    Message = $"Tool '{task.ToolName}' not found"
                };
            }

            var tool = _toolFactory.GetTool(task.ToolName);
            if (previousResults != null && previousResults.Count > 0)
            {
                // Add previous results to the task data
                if (task.Data == null)
                    task.Data = new Dictionary<string, object>();
                
                task.Data["_previousResults"] = previousResults;
            }
            return await tool.ExecuteAsync(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing task with tool: {ToolName}", task.ToolName);
            return new ToolResult
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }
    
    public string GetAllToolsDescription()
    {
        return _toolFactory.GetAllToolsDescription();
    }
}
