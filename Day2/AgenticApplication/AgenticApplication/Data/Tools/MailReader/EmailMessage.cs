namespace AgenticApplication.Tools;

public class EmailMessage
{
    public string Id { get; set; } = string.Empty;
    public string ThreadId { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Snippet { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public List<string> Labels { get; set; } = new List<string>();
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
}