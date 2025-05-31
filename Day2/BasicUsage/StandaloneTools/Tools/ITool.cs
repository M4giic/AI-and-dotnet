using System.Threading.Tasks;
using Tools.Models;

namespace Tools.Tools;

public interface ITool
{
    string ToolName { get; }
    string GetDescription();
    string GetDetailedDescription();
    Task<ToolResult> ExecuteAsync(ToolTask task);
}
