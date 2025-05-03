using System.Reflection;
using AgenticApplication.Tools;

namespace AgenticApplication.Services;

public class ToolFactory : IToolFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _toolTypes = new (StringComparer.OrdinalIgnoreCase);
    private string ToolsDescription;


    public ToolFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        DiscoverToolTypes();
    }
    
    private void DiscoverToolTypes()
    {
        // Get all types from the current assembly
        var assembly = Assembly.GetExecutingAssembly();
        
        // Find all classes that implement ITool
        var toolTypes = assembly.GetTypes()
            .Where(type => typeof(ITool).IsAssignableFrom(type) && 
                           !type.IsInterface && 
                           !type.IsAbstract);
        
        foreach (var toolType in toolTypes)
        {
            // Create a temporary instance to get the tool name
            var tool = (ITool)_serviceProvider.GetRequiredService(toolType);
            _toolTypes[tool.ToolName.ToLower()] = toolType;
        }
    }

    public ITool GetTool(string toolName)
    {
        return toolName.ToLower() switch
        {
            "email-sender" => _serviceProvider.GetRequiredService<EmailSenderTool>(),
            "gmail-reader" => _serviceProvider.GetRequiredService<GmailReaderTool>(),
            "final" => _serviceProvider.GetRequiredService<FinalResonseTool>(),
            _ => throw new ArgumentException($"Tool '{toolName}' not found")
        };
    }

    public bool HasTool(string toolName)
    {
        return toolName.ToLower() switch
        {
            "email-sender" => true,
            "gmail-reader" => true,
            "final" => true,
            _ => false
        };
    }

    public string GetAllToolsDescription()
    {
        if (String.IsNullOrWhiteSpace(ToolsDescription))
        {
            var toolDescriptions = GetAllTools()
                .Select(tool => $"{tool.ToolName}: {tool.GetDescription()}")
                .ToList();

            ToolsDescription = String.Join(Environment.NewLine, toolDescriptions);
        }
       
        return ToolsDescription;
    }
    
    
    private IEnumerable<ITool> GetAllTools()
    {
        foreach (var toolType in _toolTypes.Values)
        {
            yield return (ITool)_serviceProvider.GetService(toolType);
        }
    }
}