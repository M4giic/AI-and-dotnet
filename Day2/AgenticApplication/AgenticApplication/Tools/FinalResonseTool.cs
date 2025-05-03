using AgenticApplication.Data.Models;
using AgenticApplication.Services;
using Microsoft.Extensions.Options;

namespace AgenticApplication.Tools;

public class FinalResonseTool : ITool
{
    private readonly IOpenAiClientService _openAiClientService;
    private readonly ILogger<FinalResonseTool> _logger;

    public FinalResonseTool(
        IOpenAiClientService openAiClientService,
        ILogger<FinalResonseTool> logger)
    {
        _openAiClientService = openAiClientService;
        _logger = logger;
    }
    public string ToolName => "final";

    public string GetDescription()
    {
        return "Generates coherent and personalized responses based on information gathered from other tools. " +
               "This tool should be the final step in a sequence, synthesizing all collected data into a natural language response.";
    }

    public async Task<ToolResult> ExecuteAsync(ToolTask task)
    {
        try
        {
            _logger.LogInformation("Executing Response Generator Tool with instruction: {Instruction}", task.Instruction);

            // Extract the required parameters
            // if (!TryExtractParameters(task.Data, out var userQuery, out var previousResults, out var preferences, out var responseFormat))
            // {
            //     return new ToolResult
            //     {
            //         Success = false,
            //         Message = "Missing required parameters for response generation"
            //     };
            // }

            // Generate the response
            // var (response, metadata) = await GenerateResponseAsync(userQuery, previousResults, preferences, responseFormat);

            return new ToolResult
            {
                Success = true,
                Message = "Response generated successfully",
                ResultData = new Dictionary<string, object>
                {
                    // { "response", response },
                    // { "responseFormat", responseFormat },
                    // { "metadata", metadata },
                    { "timestamp", DateTime.UtcNow }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing Response Generator Tool");
            return new ToolResult
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }
}