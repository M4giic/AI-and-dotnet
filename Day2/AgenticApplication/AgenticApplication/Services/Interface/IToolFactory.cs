using AgenticApplication.Tools;

namespace AgenticApplication.Services;

public interface IToolFactory
{
    ITool GetTool(string toolName);
    bool HasTool(string toolName);
    string GetAllToolsDescription();
}