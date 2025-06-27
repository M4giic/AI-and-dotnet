using Tools.Agent.Services;
using Tools.Models;
using Tools.Tools;

namespace Tools.Agent;

public class AgentProcessor
{
    private readonly EmailSettings _emailSettings;
    private readonly AgentSettings _agentSettings;
    private readonly Dictionary<string, ITool> _tools = new Dictionary<string, ITool>();
    private readonly ActionService _actionService;

    public AgentProcessor(EmailSettings emailSettings, AgentSettings agentSettings)
    {
        _emailSettings = emailSettings;
        _agentSettings = agentSettings;
        _actionService = new ActionService(agentSettings.OpenAIApiKey);
        InitializeTools();
    }

    private void InitializeTools()
    {
        var emailTool = new MailSenderTool(_agentSettings.OpenAIApiKey, _emailSettings);
        _tools.Add(emailTool.ToolName.ToLower(), emailTool);
    }

    // Use ActionService to create an action plan (calls OpenAI)
    public async Task<ActionSequence> CreateActionSequenceAsync(string userPrompt)
    {
        return await _actionService.PrepareActionSequenceAsync(userPrompt);
    }

    // Step: Tool Use - Execute the actual tools
    public async Task<ToolResult> ExecuteToolAsync(ToolTask task)
    {
        Console.WriteLine($"Tool Use step: Executing tool {task.ToolName}");

        if (_tools.TryGetValue(task.ToolName.ToLower(), out var tool))
        {
            return await tool.ExecuteAsync(task);
        }
        else
        {
            return new ToolResult
            {
                Success = false,
                Message = $"Unknown tool: {task.ToolName}"
            };
        }
    }
}
