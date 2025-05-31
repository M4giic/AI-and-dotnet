using System.Collections.Generic;

namespace Tools.Models;

public class ToolTask
{
    public string ToolName { get; set; } = string.Empty;
    public string Instruction { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();  
}

public class ToolResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, object> ResultData { get; set; } = new Dictionary<string, object>();
}

public class ActionSequence
{
    public List<ToolTask> Actions { get; set; } = new List<ToolTask>();
    public string Description { get; set; } = string.Empty;
}
