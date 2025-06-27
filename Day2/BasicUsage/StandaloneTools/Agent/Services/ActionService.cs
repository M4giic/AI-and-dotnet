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
    private readonly Dictionary<string, string> _toolDescriptions;
    
    public ActionService(string apiKey)
    {
        _chatClient = new ChatClient(model: "gpt-4.1", apiKey: apiKey);
        
        // Initialize tool descriptions
        _toolDescriptions = new Dictionary<string, string>
        {
            ["EmailTool"] = "Sends emails to specified recipients"
        };
    }
    
    public async Task<ActionSequence> PrepareActionSequenceAsync(string userPrompt)
    {
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(GetActionPrompt()),
            new UserChatMessage(userPrompt)
        };
        
        ChatCompletion completion = _chatClient.CompleteChat(messages);
        // Extract the content from the completion correctly
        var content = completion.Content.FirstOrDefault()?.Text ?? string.Empty;
        
        Console.WriteLine("Action sequence prepared.");
        
        try
        {
            // Parse the action result into an ActionSequence
            var actionSequence = ParseActionSequence(content);
            Console.WriteLine($"Created action sequence with {actionSequence.Actions.Count} actions.");
            return actionSequence;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing action sequence: {ex.Message}");
            throw;
        }
    }
    
    private string GetActionPrompt()
    {
        return $@"You are an AI assistant that helps prepare tasks for tools.

You have tools available to you, each with a specific purpose. Here are the tools you can use:
{string.Join("\n", _toolDescriptions.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}
When preparing a task for a tool, you must provide the necessary information in JSON format.

Based on the user's instruction, prepare a simple JSON object with the key information needed to execute this task.
Do not attempt to execute the task yourself, just prepare the necessary information.

Respond with a JSON object in this format:
{{
  ""taskDescription"": ""Brief description of what needs to be done"",
  ""toolToUse"": ""{{toolName}}""
}}

<Rules>
Obey this rules:
- If task is given in different language then remain this language.
</Rules>

Respond with a JSON object containing the relevant information for this tool.
Always return valid JSON that can be parsed programmatically.
";
    }
    
    private ActionSequence ParseActionSequence(string content)
    {
        try
        {
            // Extract JSON content if it's embedded in other text
            string jsonContent = JsonHelper.ExtractJson(content);

            // Parse the new format: { "taskDescription": "...", "toolToUse": "..." }
            var doc = JsonDocument.Parse(jsonContent);
            var root = doc.RootElement;

            var taskDescription = root.GetProperty("taskDescription").GetString();
            var toolToUse = root.GetProperty("toolToUse").GetString();

            var actions = new List<ToolTask>
            {
                new ToolTask
                {
                    ToolName = toolToUse,
                    Instruction = taskDescription
                }
            };

            return new ActionSequence
            {
                Actions = actions
            };
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
            Console.WriteLine($"Content received: {content}");

            // Return an empty action sequence on error
            return new ActionSequence
            {
                Actions = new List<ToolTask>()
            };
        }
    }
}
