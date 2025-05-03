using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using AgenticApplication.Data.Models;
using AgenticApplication.Data.Prompts;
using AgenticApplication.Data.Request;
using AgenticApplication.Data.Response;
using AgenticApplication.Data.Settings;
using Microsoft.Extensions.Options;

namespace AgenticApplication.Services;

public class AiAgentService : IAiAgentService
{
    private readonly IToolingService _toolingService;
    private readonly IOpenAiClientService _openAiClientService;
    private readonly IOptions<AgentSettings> _agentSettings;
    private readonly ILogger<AiAgentService> _logger;

    public AiAgentService(
        IToolingService toolingService,
        IOptions<AgentSettings> agentSettings,
        ILogger<AiAgentService> logger, 
        IOpenAiClientService openAiClientService)
    {
        _toolingService = toolingService;
        _agentSettings = agentSettings;
        _logger = logger;
        _openAiClientService = openAiClientService;
    }

    public async Task<ChatResponse> ProcessMessageAsync(ChatRequest request)
    {
        try
        {
            _logger.LogInformation("Processing chat message: {Message}", request.Message);

            // 1. Generate a task sequence plan using OpenAI
            var taskSequence = await GenerateTaskSequenceAsync(request);

            if (taskSequence == null || !taskSequence.Steps.Any())
            {
                return new ChatResponse
                {
                    Message = "I'm not sure how to help with that request. Could you provide more details?"
                };
            }

            // 2. Execute each task in the sequence
            var actionResults = new List<ActionResult>();
            var responseBuilder = new StringBuilder();
            var toolResults = new Dictionary<string, ToolResult>();

            foreach (var step in taskSequence.Steps)
            {
                // Execute the current step
                var task = new ToolTask
                {
                    ToolName = step.ToolName,
                    Instruction = step.Instruction,
                    Data = step.Parameters
                };

                var result = await _toolingService.ExecuteTaskAsync(task, toolResults);

                // Add to action results
                var actionResult = new ActionResult
                {
                    ActionType = step.ToolName,
                    Description = step.Instruction,
                    Data = new Dictionary<string, object>
                    {
                        { "success", result.Success },
                        { "message", result.Message }
                    }
                };
                toolResults[step.ToolName] = result;

                // Add all result data
                foreach (var item in result.ResultData)
                {
                    actionResult.Data[item.Key] = item.Value;
                }

                actionResults.Add(actionResult);

                // Append to response builder
                if (result.Success)
                {
                    responseBuilder.AppendLine($"✓ {step.Instruction}");
                }
                else if (step.IsRequired)
                {
                    responseBuilder.AppendLine($"✗ {step.Instruction} - {result.Message}");
                }
            }

            // 3. Generate a final response with OpenAI based on the actions taken
            var finalResponse = await GenerateFinalResponseAsync(request, actionResults);

            return new ChatResponse
            {
                Message = finalResponse,
                Actions = actionResults
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat message");
            return new ChatResponse
            {
                Message = "I encountered an error while processing your request. Please try again."
            };
        }
    }

    private async Task<TaskSequence> GenerateTaskSequenceAsync(ChatRequest request)
    {
        try
        {
            var planningPromptData = new PlanningPromptData()
            {
                Tools = _toolingService.GetAllToolsDescription()
            };

            if (request.Context != null && request.Context.Count > 0)
            {
                var conext = new StringBuilder();
                foreach (var item in request.Context)
                {
                    conext.AppendLine($"- {item.Key}: {item.Value}");
                }
                planningPromptData.Context = conext.ToString();
            }
            
            var response = await _openAiClientService.GetUserAndSystemCompletionAsync(nameof(PlanningPrompt),request.Message, planningPromptData);
            
            // Extract JSON from the response - it might be wrapped in markdown code blocks
            var jsonMatch = Regex.Match(response, @"\{[\s\S]*\}");
            if (jsonMatch.Success)
            {
                var json = jsonMatch.Value;
                try
                {
                    var taskSequence = JsonSerializer.Deserialize<TaskSequence>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return taskSequence;
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to parse task sequence JSON: {Json}", json);
                }
            }
            else
            {
                _logger.LogWarning("Could not find JSON in OpenAI response: {Content}", response);
            }
          

            return new TaskSequence { Steps = new List<SequenceStep>() };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating task sequence");
            return new TaskSequence { Steps = new List<SequenceStep>() };
        }
    }

    private async Task<string> GenerateFinalResponseAsync(ChatRequest request, List<ActionResult> actions)
    {
        try
        {
            var actionsJson = JsonSerializer.Serialize(actions);
            
            var resultPromptData = new ResultPromptData
            {
                ActionsJson = actionsJson,
                UserMessage = request.Message
            };
            // Call OpenAI to generate the response
            var response = await _openAiClientService.GetUserAndSystemCompletionAsync(nameof(ResultPrompt),"Generate summarization", resultPromptData);


            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating final response");
            return "I've completed the requested tasks. Let me know if you need anything else.";
        }
    }
}
