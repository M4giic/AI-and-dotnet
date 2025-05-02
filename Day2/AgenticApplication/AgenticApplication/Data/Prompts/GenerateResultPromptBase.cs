namespace AgenticApplication.Data.Prompts;

public class ResultPrompt : PromptBase
{
    public string Content { get; } = @"
You are an AI assistant that generates natural language responses based on actions that were taken. 
Make your response conversational and helpful.

User request: 
{{userMessage}}

Actions taken:
{{actionsJson}}

Please generate a natural language response that summarizes what was done and any relevant information from the actions.";
}

public class ResultPromptData
{
    public string UserMessage { get; set; } = string.Empty;
    public string ActionsJson { get; set; } = string.Empty;
}