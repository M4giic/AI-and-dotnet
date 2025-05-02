namespace AgenticApplication.Data.Request;

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, object> Context { get; set; } = new Dictionary<string, object>();   
}