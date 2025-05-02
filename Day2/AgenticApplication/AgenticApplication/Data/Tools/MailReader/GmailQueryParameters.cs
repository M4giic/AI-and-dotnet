namespace AgenticApplication.Tools;

public class GmailQueryParameters
{
    public int MaxResults { get; set; } = 10;
    public bool IncludeBody { get; set; } = true;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string After { get; set; } = string.Empty;
    public string Before { get; set; } = string.Empty;
    public bool HasAttachment { get; set; } = false;
    public bool IsUnread { get; set; } = false;
    public string Label { get; set; } = string.Empty;
    public string SearchText { get; set; } = string.Empty;
}