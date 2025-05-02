namespace AgenticApplication.Data.Models;

public class SequenceStep
{
    public string ToolName { get; set; } = string.Empty;
    public string Instruction { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    public bool IsRequired { get; set; } = true;
    public string DependsOn { get; set; } = string.Empty;
}