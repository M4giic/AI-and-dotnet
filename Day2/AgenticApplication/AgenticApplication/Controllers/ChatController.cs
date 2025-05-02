using AgenticApplication.Data.Request;
using AgenticApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgenticApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IAiAgentService _agentService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IAiAgentService agentService, ILogger<ChatController> logger)
    {
        _agentService = agentService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessMessage([FromBody] ChatRequest request)
    {
        if (string.IsNullOrEmpty(request.Message))
        {
            return BadRequest("Message cannot be empty");
        }

        try
        {
            var response = await _agentService.ProcessMessageAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat message: {Message}", request.Message);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }
}