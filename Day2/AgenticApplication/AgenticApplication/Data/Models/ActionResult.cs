namespace AgenticApplication.Data.Response;

public class ActionResult
{
    public string ActionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
}