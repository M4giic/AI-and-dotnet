namespace AgenticApplication.Tools;

public class GmailSettings
{
    // Authentication options
    public string ServiceAccountKeyPath { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
        
    // Application settings
    public string ApplicationName { get; set; } = "AI Agent";
    public string UserEmail { get; set; } = string.Empty;
}