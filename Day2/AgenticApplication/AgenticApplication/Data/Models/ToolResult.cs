namespace AgenticApplication.Data.Models;

public class ToolResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, object> ResultData { get; set; } = new Dictionary<string, object>();
}