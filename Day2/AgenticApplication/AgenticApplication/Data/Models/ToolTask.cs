namespace AgenticApplication.Data.Models;

public class ToolTask
{
    public string ToolName { get; set; } = string.Empty;
    public string Instruction { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();  
}