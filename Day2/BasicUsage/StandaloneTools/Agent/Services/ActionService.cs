using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI.Chat;
using Tools.Models;
using Tools.Tools;

namespace Tools.Agent.Services;

public class ActionService
{
    private readonly ChatClient _chatClient;
    private readonly string _defaultEmailRecipient;
    private readonly Dictionary<string, string> _toolDescriptions;
    
    public ActionService(string apiKey, string defaultEmailRecipient = null)
    {
        _chatClient = new ChatClient(model: "gpt-4.1", apiKey: apiKey);
        _defaultEmailRecipient = defaultEmailRecipient ?? "jendraas@gmail.com";
        
        // Initialize tool descriptions
        _toolDescriptions = new Dictionary<string, string>
        {
            ["EmailTool"] = "Sends emails to specified recipients"
        };
    }
    
    public async Task<ActionSequence> PrepareActionSequenceAsync(ToolTask task)
    {
        Console.WriteLine($"Action step: Preparing action sequence for task: {task.ToolName} - {task.Instruction}");
        
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(GetActionPrompt(task.ToolName)),
            new UserChatMessage(task.Instruction)
        };
        
        ChatCompletion completion = _chatClient.CompleteChat(messages);
        // Extract the content from the completion correctly
        var content = completion.Content.FirstOrDefault()?.Text ?? string.Empty;
        
        Console.WriteLine("Action sequence prepared.");
        
        try
        {
            // Parse the action result into an ActionSequence
            var actionSequence = ParseActionSequence(content, task);
            
            // If this is an email task, ensure the recipient is set to the default email
            if (task.ToolName.Equals("EmailTool", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var action in actionSequence.Actions)
                {
                    // Override the recipient with the default email
                    action.Data["to"] = _defaultEmailRecipient;
                    Console.WriteLine($"Email recipient set to default: {_defaultEmailRecipient}");
                }
            }
            
            Console.WriteLine($"Created action sequence with {actionSequence.Actions.Count} actions.");
            return actionSequence;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing action sequence: {ex.Message}");
            throw;
        }
    }
    
    private string GetActionPrompt(string toolName)
    {
        string toolDescription = _toolDescriptions.TryGetValue(toolName, out var desc) 
            ? desc 
            : "Performs actions based on instructions";
            
        if (toolName.Equals("EmailTool", StringComparison.OrdinalIgnoreCase))
        {
            return $@"You are an AI assistant that helps prepare tasks for tools.

You'll be using the {toolName} which {toolDescription}.

Based on the user's instruction, prepare a simple JSON object with the key information needed to execute this task.
Do not attempt to execute the task yourself, just prepare the necessary information.

Respond with a JSON object in this format:
{{
  ""taskDescription"": ""Brief description of what needs to be done"",
  ""toolToUse"": ""{{toolName}}""
}}

Only include these fields. Always return valid JSON that can be parsed programmatically.
Do not include any explanations outside the JSON structure.";
        }
        
        return $@"You are an AI assistant that helps prepare tasks for tools.

You'll be using the {toolName} which {toolDescription}.

Based on the user's instruction, prepare a simple JSON object with the key information needed to execute this task.
Do not attempt to execute the task yourself, just prepare the necessary information.

Respond with a JSON object containing the relevant information for this tool.
Always return valid JSON that can be parsed programmatically.";
    }
    
    private ActionSequence ParseActionSequence(string content, ToolTask originalTask)
    {
        try
        {
            // Extract JSON content if it's embedded in other text
            string jsonContent = JsonHelper.ExtractJson(content);
            
            var actionSequence = new ActionSequence
            {
                Description = $"Action sequence for {originalTask.ToolName}"
            };
            
            // For EmailTool, we deserialize the JSON to get email details
            if (originalTask.ToolName.Equals("EmailTool", StringComparison.OrdinalIgnoreCase))
            {
                // Deserialize the JSON into a dictionary
                var emailData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                var toolTask = new ToolTask
                {
                    ToolName = originalTask.ToolName,
                    Instruction = originalTask.Instruction,
                    Data = new Dictionary<string, object>()
                };
                
                // Add each key-value pair to the tool task data
                foreach (var kvp in emailData)
                {
                    toolTask.Data[kvp.Key] = kvp.Value;
                }
                
                actionSequence.Actions.Add(toolTask);
            }
            
            return actionSequence;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
            Console.WriteLine($"Content received: {content}");
            
            // Create a default action sequence with the original task
            var actionSequence = new ActionSequence
            {
                Description = $"Action sequence for {originalTask.ToolName} (fallback)"
            };
            
            actionSequence.Actions.Add(new ToolTask
            {
                ToolName = originalTask.ToolName,
                Instruction = originalTask.Instruction,
                Data = new Dictionary<string, object>()
            });
            
            return actionSequence;
        }
    }
}
