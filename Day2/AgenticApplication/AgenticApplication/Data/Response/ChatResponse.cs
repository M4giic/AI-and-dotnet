namespace AgenticApplication.Data.Response;

public class ChatResponse
{
    public string Message { get; set; } = string.Empty;
    public List<ActionResult> Actions { get; set; } = new List<ActionResult>();
}