using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI.Chat;
using Tools.Models;

namespace Tools.Agent.Services;

public class PlanningService
{
    private readonly ChatClient _chatClient;
    
    public PlanningService(string apiKey)
    {
        _chatClient = new ChatClient(model: "gpt-4.1", apiKey: apiKey);
    }
    
    public async Task<List<ToolTask>> PlanAsync(string userPrompt)
    {
        Console.WriteLine("Planning step: Analyzing user input...");
        
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(GetPlanningPrompt()),
            new UserChatMessage(userPrompt)
        };
        
        ChatCompletion completion = _chatClient.CompleteChat(messages);
        var content = completion.Content.FirstOrDefault()?.Text ?? string.Empty;
        
        Console.WriteLine("Planning response received.");
        
        try
        {
            // Parse the planning result into a list of ToolTasks
            var tasks = ParsePlannedTasks(content);
            Console.WriteLine($"Identified {tasks.Count} tasks to execute.");
            return tasks;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing planning result: {ex.Message}");
            throw;
        }
    }
    
    private string GetPlanningPrompt()
    {
        return @"You are an AI planning assistant that helps identify tasks from user instructions.
Your job is to analyze the user's request and identify what needs to be done.

Your task is to understand what the user wants to accomplish and create a simple plan.
Focus on the high-level goals, not on specific implementation details or tools.

Respond with a JSON array of task objects in the following format:
[
  {
    ""taskType"": ""TaskCategory"",
    ""instruction"": ""Brief description of what to do""
  }
]

Task categories you can use:
- Communication: For sending messages, emails, or notifications
- Information: For retrieving or providing information
- Scheduling: For calendar and time management tasks

Example response for a communication request:
[
  {
    ""taskType"": ""Communication"",
    ""instruction"": ""Send a meeting reminder to the team""
  }
]

Keep your response focused on just identifying the user's goals, without specifying implementation details.
Always return valid JSON that can be parsed programmatically.";
    }
    
    private List<ToolTask> ParsePlannedTasks(string content)
    {
        try
        {
            // Extract JSON content if it's embedded in other text
            string jsonContent = JsonHelper.ExtractJson(content);
            
            // Deserialize the JSON array into a list of task objects
            var taskDtos = JsonSerializer.Deserialize<List<PlanningTaskDto>>(jsonContent, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            // Convert DTOs to ToolTask objects
            var tasks = new List<ToolTask>();
            foreach (var taskDto in taskDtos)
            {
                var toolName = MapTaskTypeToTool(taskDto.TaskType);
                
                var task = new ToolTask
                {
                    ToolName = toolName,
                    Instruction = taskDto.Instruction,
                    Data = new Dictionary<string, object>()
                };
                tasks.Add(task);
            }
            
            return tasks;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
            Console.WriteLine($"Content received: {content}");
            
            // Fallback to basic text parsing if JSON parsing fails
            return ParsePlannedTasksTextFallback(content);
        }
    }
    
    private string MapTaskTypeToTool(string taskType)
    {
        return taskType.ToLower() switch
        {
            "communication" => "EmailTool",
            "information" => "EmailTool", // Currently mapping to EmailTool as it's our only tool
            "scheduling" => "EmailTool",  // Currently mapping to EmailTool as it's our only tool
            _ => "EmailTool" // Default to EmailTool for unknown task types
        };
    }
    
    // Fallback text parser in case JSON parsing fails
    private List<ToolTask> ParsePlannedTasksTextFallback(string content)
    {
        var tasks = new List<ToolTask>();
        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        ToolTask currentTask = null;
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            if (trimmedLine.StartsWith("TOOL:", StringComparison.OrdinalIgnoreCase))
            {
                if (currentTask != null)
                {
                    tasks.Add(currentTask);
                }
                
                currentTask = new ToolTask
                {
                    ToolName = trimmedLine.Substring("TOOL:".Length).Trim(),
                    Data = new Dictionary<string, object>()
                };
            }
            else if (trimmedLine.StartsWith("INSTRUCTION:", StringComparison.OrdinalIgnoreCase) && currentTask != null)
            {
                currentTask.Instruction = trimmedLine.Substring("INSTRUCTION:".Length).Trim();
            }
        }
        
        if (currentTask != null && !string.IsNullOrEmpty(currentTask.Instruction))
        {
            tasks.Add(currentTask);
        }
        
        return tasks;
    }
    
    // DTO class for JSON deserialization of planning tasks
    private class PlanningTaskDto
    {
        public string TaskType { get; set; }
        public string Instruction { get; set; }
    }
}
