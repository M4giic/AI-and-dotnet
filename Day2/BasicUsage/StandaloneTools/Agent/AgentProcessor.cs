using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools.Agent.Services;
using Tools.Models;
using Tools.Tools;

namespace Tools.Agent;

public class AgentProcessor
{
    private readonly EmailSettings _settings;
    private readonly Dictionary<string, ITool> _tools = new Dictionary<string, ITool>();
    
    // Services
    private readonly PlanningService _planningService;
    private readonly ActionService _actionService;
    
    public AgentProcessor(EmailSettings settings)
    {
        _settings = settings;
        
        // Initialize services
        _planningService = new PlanningService(settings.ApiKey);
        _actionService = new ActionService(settings.ApiKey, "jendraas@gmail.com");
        
        // Initialize tools
        InitializeTools();
    }
    
    private void InitializeTools()
    {
        // Initialize email tool
        var emailTool = new MailSenderTool();
        emailTool.SetApiKey(_settings.ApiKey);
        emailTool.UpdateSettings(
            username: _settings.Username,
            password: _settings.Password,
            senderName: _settings.SenderName,
            senderEmail: _settings.SenderEmail
        );
        
        // Register tools
        _tools.Add(emailTool.ToolName.ToLower(), emailTool);
    }
    
    // Step 1: Planning - Analyze user input to identify tasks
    public async Task<List<ToolTask>> PlanAsync(string userPrompt)
    {
        return await _planningService.PlanAsync(userPrompt);
    }
    
    // Step 2: Action - Prepare a structured action sequence
    public async Task<ActionSequence> PrepareActionSequenceAsync(ToolTask task)
    {
        return await _actionService.PrepareActionSequenceAsync(task);
    }
    
    // Step 3: Tool Use - Execute the actual tools
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

