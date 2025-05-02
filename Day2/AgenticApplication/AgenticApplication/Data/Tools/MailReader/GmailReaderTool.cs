using System.Text;
using System.Text.RegularExpressions;
using AgenticApplication.Data.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Options;

namespace AgenticApplication.Tools;

public class GmailReaderTool : ITool
{
    public string ToolName => "gmail-reader";

    private readonly GmailSettings _gmailSettings;
    private readonly ILogger<GmailReaderTool> _logger;

    public GmailReaderTool(
        IOptions<GmailSettings> gmailSettings,
        ILogger<GmailReaderTool> logger)
    {
        _gmailSettings = gmailSettings.Value;
        _logger = logger;
    }

    public string GetDescription()
    {
        return "Reads and searches emails from Gmail. Use this tool to find, retrieve, and analyze emails " +
               "based on criteria like sender, subject, date, labels, or content keywords.";
    }

    public async Task<ToolResult> ExecuteAsync(ToolTask task)
    {
        try
        {
            _logger.LogInformation("Executing Gmail Reader Tool with instruction: {Instruction}", task.Instruction);

            // Extract parameters from task data
            var queryParams = ExtractQueryParameters(task);

            // Initialize Gmail service
            var gmailService = await InitializeGmailServiceAsync();
            if (gmailService == null)
            {
                return new ToolResult
                {
                    Success = false,
                    Message = "Failed to initialize Gmail service"
                };
            }

            // Build Gmail query
            var query = BuildGmailQuery(queryParams);
            _logger.LogInformation("Gmail query: {Query}", query);

            // Fetch emails
            var emails = await FetchEmailsAsync(gmailService, query, queryParams.MaxResults);

            // Process emails based on the instruction
            var processedEmails = ProcessEmails(emails, queryParams);

            return new ToolResult
            {
                Success = true,
                Message = $"Successfully retrieved {processedEmails.Count} emails",
                ResultData = new Dictionary<string, object>
                {
                    { "emails", processedEmails },
                    { "query", query },
                    { "totalCount", processedEmails.Count }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing Gmail Reader Tool");
            return new ToolResult
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }

    private GmailQueryParameters ExtractQueryParameters(ToolTask task)
    {
        var parameters = new GmailQueryParameters();

        if (task.Data != null)
        {
            // Extract query parameters from task data
            if (task.Data.TryGetValue("maxResults", out var maxResults) && maxResults is int maxResultsInt)
            {
                parameters.MaxResults = maxResultsInt;
            }
            else if (task.Data.TryGetValue("maxResults", out var maxResultsStr) && 
                     maxResultsStr is string maxResultsString &&
                     int.TryParse(maxResultsString, out var parsedMaxResults))
            {
                parameters.MaxResults = parsedMaxResults;
            }

            if (task.Data.TryGetValue("includeBody", out var includeBody) && includeBody is bool includeBodyBool)
            {
                parameters.IncludeBody = includeBodyBool;
            }

            if (task.Data.TryGetValue("after", out var after) && after is string afterStr)
            {
                parameters.After = afterStr;
            }

            if (task.Data.TryGetValue("before", out var before) && before is string beforeStr)
            {
                parameters.Before = beforeStr;
            }

            if (task.Data.TryGetValue("from", out var from) && from is string fromStr)
            {
                parameters.From = fromStr;
            }

            if (task.Data.TryGetValue("to", out var to) && to is string toStr)
            {
                parameters.To = toStr;
            }

            if (task.Data.TryGetValue("subject", out var subject) && subject is string subjectStr)
            {
                parameters.Subject = subjectStr;
            }

            if (task.Data.TryGetValue("hasAttachment", out var hasAttachment) && hasAttachment is bool hasAttachmentBool)
            {
                parameters.HasAttachment = hasAttachmentBool;
            }

            if (task.Data.TryGetValue("isUnread", out var isUnread) && isUnread is bool isUnreadBool)
            {
                parameters.IsUnread = isUnreadBool;
            }

            if (task.Data.TryGetValue("label", out var label) && label is string labelStr)
            {
                parameters.Label = labelStr;
            }

            if (task.Data.TryGetValue("searchText", out var searchText) && searchText is string searchTextStr)
            {
                parameters.SearchText = searchTextStr;
            }
        }

        // Apply default max results if not specified
        if (parameters.MaxResults <= 0)
        {
            parameters.MaxResults = 10;
        }

        return parameters;
    }

    private async Task<GmailService> InitializeGmailServiceAsync()
    {
        try
        {
            // Determine which authentication method to use
            if (!string.IsNullOrEmpty(_gmailSettings.ServiceAccountKeyPath))
            {
                // Use service account authentication
                return await InitializeWithServiceAccountAsync();
            }
            else if (!string.IsNullOrEmpty(_gmailSettings.ClientId) && 
                     !string.IsNullOrEmpty(_gmailSettings.ClientSecret) && 
                     !string.IsNullOrEmpty(_gmailSettings.RefreshToken))
            {
                // Use OAuth2 authentication with refresh token
                return InitializeWithOAuth2RefreshToken();
            }
            else if (!string.IsNullOrEmpty(_gmailSettings.AccessToken))
            {
                // Use direct access token (least preferred, as it expires)
                return InitializeWithAccessToken();
            }
            else
            {
                _logger.LogError("No valid Gmail authentication method configured");
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Gmail service");
            return null;
        }
    }

    private async Task<GmailService> InitializeWithServiceAccountAsync()
    {
        try
        {
            var credential = GoogleCredential.FromFile(_gmailSettings.ServiceAccountKeyPath)
                .CreateScoped(GmailService.Scope.GmailReadonly)
                .CreateWithUser(_gmailSettings.UserEmail);

            return new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _gmailSettings.ApplicationName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing Gmail service with service account");
            return null;
        }
    }

    private GmailService InitializeWithOAuth2RefreshToken()
    {
        try
        {
            var clientSecrets = new ClientSecrets
            {
                ClientId = _gmailSettings.ClientId,
                ClientSecret = _gmailSettings.ClientSecret
            };

            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets,
                new[] { GmailService.Scope.GmailReadonly },
                _gmailSettings.UserEmail,
                CancellationToken.None).Result;

            return new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _gmailSettings.ApplicationName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing Gmail service with OAuth2 refresh token");
            return null;
        }
    }

    private GmailService InitializeWithAccessToken()
    {
        try
        {
            var credential = GoogleCredential.FromAccessToken(_gmailSettings.AccessToken)
                .CreateScoped(GmailService.Scope.GmailReadonly);

            return new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _gmailSettings.ApplicationName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing Gmail service with access token");
            return null;
        }
    }

    private string BuildGmailQuery(GmailQueryParameters parameters)
    {
        var queryParts = new List<string>();

        if (!string.IsNullOrEmpty(parameters.From))
        {
            queryParts.Add($"from:{parameters.From}");
        }

        if (!string.IsNullOrEmpty(parameters.To))
        {
            queryParts.Add($"to:{parameters.To}");
        }

        if (!string.IsNullOrEmpty(parameters.Subject))
        {
            queryParts.Add($"subject:{parameters.Subject}");
        }

        if (!string.IsNullOrEmpty(parameters.After))
        {
            queryParts.Add($"after:{parameters.After}");
        }

        if (!string.IsNullOrEmpty(parameters.Before))
        {
            queryParts.Add($"before:{parameters.Before}");
        }

        if (parameters.HasAttachment)
        {
            queryParts.Add("has:attachment");
        }

        if (parameters.IsUnread)
        {
            queryParts.Add("is:unread");
        }

        if (!string.IsNullOrEmpty(parameters.Label))
        {
            queryParts.Add($"label:{parameters.Label}");
        }

        if (!string.IsNullOrEmpty(parameters.SearchText))
        {
            queryParts.Add(parameters.SearchText);
        }

        return string.Join(" ", queryParts);
    }

    private async Task<List<Message>> FetchEmailsAsync(GmailService service, string query, int maxResults)
    {
        var result = new List<Message>();
        string pageToken = null;
        
        try
        {
            do
            {
                var request = service.Users.Messages.List(_gmailSettings.UserEmail);
                request.Q = query;
                request.MaxResults = maxResults;
                request.PageToken = pageToken;

                var response = await request.ExecuteAsync();
                
                if (response.Messages != null)
                {
                    foreach (var message in response.Messages)
                    {
                        // Get full message details
                        var fullMessage = await service.Users.Messages.Get(_gmailSettings.UserEmail, message.Id).ExecuteAsync();
                        result.Add(fullMessage);
                        
                        if (result.Count >= maxResults)
                        {
                            return result;
                        }
                    }
                }

                pageToken = response.NextPageToken;
            } 
            while (pageToken != null && result.Count < maxResults);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching emails from Gmail");
            throw;
        }
    }

    private List<EmailMessage> ProcessEmails(List<Message> messages, GmailQueryParameters parameters)
    {
        var processedEmails = new List<EmailMessage>();

        foreach (var message in messages)
        {
            try
            {
                var email = new EmailMessage
                {
                    Id = message.Id,
                    ThreadId = message.ThreadId,
                    Snippet = message.Snippet,
                    Labels = message.LabelIds?.ToList() ?? new List<string>(),
                    Headers = new Dictionary<string, string>()
                };

                // Process headers
                if (message.Payload?.Headers != null)
                {
                    foreach (var header in message.Payload.Headers)
                    {
                        email.Headers[header.Name] = header.Value;

                        // Extract common headers
                        switch (header.Name.ToLower())
                        {
                            case "from":
                                email.From = header.Value;
                                break;
                            case "to":
                                email.To = header.Value;
                                break;
                            case "subject":
                                email.Subject = header.Value;
                                break;
                            case "date":
                                if (DateTime.TryParse(header.Value, out var date))
                                {
                                    email.Date = date;
                                }
                                break;
                        }
                    }
                }

                // Extract body if requested
                if (parameters.IncludeBody)
                {
                    email.Body = ExtractBody(message);
                }

                // Add to result list
                processedEmails.Add(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing email {MessageId}", message.Id);
                // Continue with next message
            }
        }

        return processedEmails;
    }

    private string ExtractBody(Message message)
    {
        if (message.Payload == null)
        {
            return string.Empty;
        }

        // Try to get the body directly from the payload
        if (!string.IsNullOrEmpty(message.Payload.Body?.Data))
        {
            return DecodeBase64UrlSafe(message.Payload.Body.Data);
        }

        // If not found, traverse the parts recursively
        if (message.Payload.Parts != null)
        {
            return ExtractBodyFromParts(message.Payload.Parts);
        }

        return string.Empty;
    }

    private string ExtractBodyFromParts(IList<MessagePart> parts)
    {
        var textPart = parts.FirstOrDefault(p => p.MimeType == "text/plain");
        if (textPart != null && !string.IsNullOrEmpty(textPart.Body?.Data))
        {
            return DecodeBase64UrlSafe(textPart.Body.Data);
        }

        var htmlPart = parts.FirstOrDefault(p => p.MimeType == "text/html");
        if (htmlPart != null && !string.IsNullOrEmpty(htmlPart.Body?.Data))
        {
            // Convert HTML to plain text (simple version)
            var html = DecodeBase64UrlSafe(htmlPart.Body.Data);
            return ConvertHtmlToPlainText(html);
        }

        // Recursively check subparts
        foreach (var part in parts)
        {
            if (part.Parts != null)
            {
                var body = ExtractBodyFromParts(part.Parts);
                if (!string.IsNullOrEmpty(body))
                {
                    return body;
                }
            }
        }

        return string.Empty;
    }

    private string DecodeBase64UrlSafe(string base64UrlSafe)
    {
        // Convert from URL-safe Base64 to regular Base64
        string base64 = base64UrlSafe.Replace('-', '+').Replace('_', '/');
        
        // Add padding if needed
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        // Decode
        byte[] bytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(bytes);
    }

    private string ConvertHtmlToPlainText(string html)
    {
        // Simple HTML to plain text conversion
        // Remove HTML tags
        var text = Regex.Replace(html, "<.*?>", " ");
        
        // Decode HTML entities
        text = System.Web.HttpUtility.HtmlDecode(text);
        
        // Replace multiple spaces with a single space
        text = Regex.Replace(text, @"\s+", " ");
        
        // Trim leading and trailing whitespace
        return text.Trim();
    }
}
