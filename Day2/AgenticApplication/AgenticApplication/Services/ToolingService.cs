using AgenticApplication.Data.Models;

namespace AgenticApplication.Services;

public class ToolingService : IToolingService
{
    private readonly IToolFactory _toolFactory;
    private readonly ILogger<ToolingService> _logger;

    public ToolingService(IToolFactory toolFactory, ILogger<ToolingService> logger)
    {
        _toolFactory = toolFactory;
        _logger = logger;
    }

    public async Task<ToolResult> ExecuteTaskAsync(ToolTask task)
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