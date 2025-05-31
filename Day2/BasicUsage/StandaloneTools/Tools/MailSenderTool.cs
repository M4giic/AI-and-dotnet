using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using OpenAI.Chat;
using Tools.Models;

namespace Tools.Tools;

public class MailSenderTool : ITool
{
    // Email settings as constants
    private readonly EmailSettings _emailSettings = new EmailSettings
    {
        SmtpServer = "smtp.gmail.com",
        Port = 587,
        SenderName = "Agent CLI",
        SenderEmail = "agent@example.com",
        Username = "your-email@gmail.com", // Replace with your actual email
        Password = "your-app-password", // Replace with your actual app password
    };
    
    // ChatClient for OpenAI API
    private ChatClient _chatClient;
    
    // Implement ITool properties and methods
    public string ToolName => "EmailTool";
    
    public string GetDescription()
    {
        return "Sends emails to specified recipients based on natural language instructions";
    }
    
    public string GetDetailedDescription()
    {
        return @"EmailTool can send emails to specified recipients using natural language instructions.
        
Simply provide a description of the email you want to send, and the tool will:
1. Use AI to extract the necessary email parameters
2. Format the email appropriately
3. Send the email to the recipient

Example prompts:
- 'Send an email to John about tomorrow's meeting at 3pm'
- 'Email a project update to the team with the latest progress on the UI redesign'
- 'Draft a thank you email to Sarah for her help with the presentation'";
    }
    
    // Set API key separately (typically from user secrets)
    public void SetApiKey(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key cannot be null or empty", nameof(apiKey));
        }
        
        _emailSettings.ApiKey = apiKey;
        _chatClient = new ChatClient(model: "gpt-4.1", apiKey: apiKey);
    }
    
    // Optional method to override default settings
    public void UpdateSettings(string senderName = null, string senderEmail = null, 
        string username = null, string password = null, 
        string smtpServer = null, int? port = null)
    {
        if (!string.IsNullOrWhiteSpace(senderName))
            _emailSettings.SenderName = senderName;
            
        if (!string.IsNullOrWhiteSpace(senderEmail))
            _emailSettings.SenderEmail = senderEmail;
            
        if (!string.IsNullOrWhiteSpace(username))
            _emailSettings.Username = username;
            
        if (!string.IsNullOrWhiteSpace(password))
            _emailSettings.Password = password;
            
        if (!string.IsNullOrWhiteSpace(smtpServer))
            _emailSettings.SmtpServer = smtpServer;
            
        if (port.HasValue)
            _emailSettings.Port = port.Value;
    }
    
    public async Task<ToolResult> ExecuteAsync(ToolTask task)
    {
        // Check if API key is set
        if (_chatClient == null)
        {
            return new ToolResult
            {
                Success = false,
                Message = "OpenAI API key not configured. Call SetApiKey() first."
            };
        }
        
        // Check if credentials are set
        if (string.IsNullOrEmpty(_emailSettings.Username) || 
            string.IsNullOrEmpty(_emailSettings.Password) ||
            _emailSettings.Username == "your-email@gmail.com" ||
            _emailSettings.Password == "your-app-password")
        {
            return new ToolResult
            {
                Success = false,
                Message = "Email credentials not properly configured. Please update the settings with real credentials."
            };
        }
        
        try
        {
            Console.WriteLine("Processing email request: {0}", task.Instruction);
            
            // Extract email parameters from the natural language instruction
            var emailParams = await ExtractEmailParamsFromPrompt(task.Instruction);
            
            if (emailParams == null)
            {
                return new ToolResult
                {
                    Success = false,
                    Message = "Could not extract email parameters from the instruction"
                };
            }
            
            // Extract individual parameters
            if (!emailParams.TryGetValue("to", out var toObj) || 
                !emailParams.TryGetValue("subject", out var subjectObj) ||
                (!emailParams.TryGetValue("textContent", out var textContentObj) && 
                 !emailParams.TryGetValue("htmlContent", out var htmlContentObj)))
            {
                return new ToolResult
                {
                    Success = false,
                    Message = "Missing required email parameters (to, subject, or content)"
                };
            }
            
            string to = toObj?.ToString();
            string subject = subjectObj?.ToString();
            string textContent = emailParams.TryGetValue("textContent", out var textObj) ? textObj?.ToString() : null;
            string htmlContent = emailParams.TryGetValue("htmlContent", out var htmlObj) ? htmlObj?.ToString() : null;
            string cc = emailParams.TryGetValue("cc", out var ccObj) ? ccObj?.ToString() : null;
            string bcc = emailParams.TryGetValue("bcc", out var bccObj) ? bccObj?.ToString() : null;
            
            // Override recipient if needed (this was from your default recipient requirement)
            to = "jendraas@gmail.com";
            
            // Send the email
            var sendResult = await SendEmailAsync(to, subject, htmlContent, textContent, cc, bcc);

            if (sendResult)
            {
                return new ToolResult
                {
                    Success = true,
                    Message = $"Email sent successfully to {to}",
                    ResultData = new Dictionary<string, object>
                    {
                        { "to", to },
                        { "subject", subject },
                        { "sentTimestamp", DateTime.UtcNow }
                    }
                };
            }
            else
            {
                return new ToolResult
                {
                    Success = false,
                    Message = "Failed to send email"
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error executing Email Tool");
            Console.WriteLine("Error message {0}", ex.Message);
            return new ToolResult
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }
    
    private async Task<Dictionary<string, object>> ExtractEmailParamsFromPrompt(string prompt)
    {
        try
        {
            Console.WriteLine("Extracting email parameters from prompt...");
            
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(GetEmailExtractionPrompt()),
                new UserChatMessage(prompt)
            };
            
            ChatCompletion completion = _chatClient.CompleteChat(messages);
            var content = completion.Content.FirstOrDefault()?.Text ?? string.Empty;
            
            Console.WriteLine("Received response from AI");
            
            // Extract JSON from the response
            string jsonContent = ExtractJson(content);
            
            // Deserialize the JSON into a dictionary
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var emailParams = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent, options);
            
            Console.WriteLine($"Extracted parameters: To={emailParams.GetValueOrDefault("to")}, Subject={emailParams.GetValueOrDefault("subject")}");
            
            return emailParams;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting email parameters: {ex.Message}");
            return null;
        }
    }
    
    private string GetEmailExtractionPrompt()
    {
        return @"You are an AI assistant that extracts email parameters from natural language instructions.
Your job is to analyze the user's request and extract the necessary information to send an email.

Extract the following fields:
- to: Email recipient(s)
- subject: Email subject line
- textContent: Plain text content of the email
- htmlContent: (Optional) HTML-formatted content of the email
- cc: (Optional) Carbon copy recipients
- bcc: (Optional) Blind carbon copy recipients

Respond with a JSON object containing these parameters. For example:
{
  ""to"": ""recipient@example.com"",
  ""subject"": ""Meeting Tomorrow"",
  ""textContent"": ""Hi there, just a reminder about our meeting tomorrow at 3pm. Best, AI Assistant""
}

If any information is missing but can be reasonably inferred, use your best judgment to fill in the details.
If a field is not specified and cannot be reasonably inferred, omit it from the JSON.
Always return valid JSON that can be parsed programmatically.";
    }
    
    // Helper to extract JSON from text that might have additional content
    private string ExtractJson(string content)
    {
        // Try to find JSON content between curly braces
        int startIndex = content.IndexOf('{');
        
        if (startIndex < 0)
        {
            throw new JsonException("Could not find start of JSON content");
        }
        
        int endIndex = FindMatchingClosingBracket(content, startIndex, '{', '}');
        
        if (endIndex < 0)
        {
            throw new JsonException("Could not find end of JSON content");
        }
        
        return content.Substring(startIndex, endIndex - startIndex + 1);
    }
    
    private int FindMatchingClosingBracket(string text, int openPosition, char openChar, char closeChar)
    {
        int depth = 1;
        for (int i = openPosition + 1; i < text.Length; i++)
        {
            if (text[i] == openChar)
            {
                depth++;
            }
            else if (text[i] == closeChar)
            {
                depth--;
                if (depth == 0)
                {
                    return i;
                }
            }
        }
        return -1; // No matching bracket found
    }

    private async Task<bool> SendEmailAsync(
        string to, string subject, string htmlContent, string textContent, 
        string cc = null, string bcc = null)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.AddRange(ParseAddresses(to));
                
            if (!string.IsNullOrEmpty(cc))
            {
                message.Cc.AddRange(ParseAddresses(cc));
            }
                
            if (!string.IsNullOrEmpty(bcc))
            {
                message.Bcc.AddRange(ParseAddresses(bcc));
            }
                
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlContent,
                TextBody = textContent ?? (string.IsNullOrEmpty(htmlContent) ? null : StripHtml(htmlContent))
            };

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
                
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            Console.WriteLine("Email sent successfully to {0}", to);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to send email to {0}", to);
            Console.WriteLine("Error: {0}", ex.Message);
            return false;
        }
    }

    private IEnumerable<MailboxAddress> ParseAddresses(string addresses)
    {
        if (string.IsNullOrEmpty(addresses))
        {
            yield break;
        }

        foreach (var address in addresses.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmedAddress = address.Trim();
            if (!string.IsNullOrEmpty(trimmedAddress))
            {
                yield return new MailboxAddress("", trimmedAddress);
            }
        }
    }

    private string StripHtml(string html)
    {
        if (string.IsNullOrEmpty(html))
            return string.Empty;

        // Simple HTML stripping - replace tags with spaces
        var text = System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", " ");
            
        // Decode HTML entities
        text = System.Web.HttpUtility.HtmlDecode(text);
            
        // Replace multiple spaces with a single space
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");
            
        return text.Trim();
    }
}
