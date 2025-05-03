using AgenticApplication.Data.Request;
using AgenticApplication.Data.Response;

namespace AgenticApplication.Services;

public interface IAiAgentService
{
    Task<ChatResponse> ProcessMessageAsync(ChatRequest request);
}