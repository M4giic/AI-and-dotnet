using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAI.Chat;
using Tools.Models;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;

namespace Tools.Tools;

public class MailSenderTool : ITool
{
    private ChatClient _chatClient;
    private EmailSettings _emailSettings;
    private MailjetClient _mailjetClient;

    public MailSenderTool(string apiKey, EmailSettings emailSettings)
    {
        _chatClient = new ChatClient(model: "gpt-4.1", apiKey: apiKey);
        _emailSettings = emailSettings;
        _mailjetClient = new MailjetClient(_emailSettings.ApiKey, _emailSettings.SecretKey);
    }
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
        if (string.IsNullOrEmpty(_emailSettings.ApiKey) ||
            string.IsNullOrEmpty(_emailSettings.SecretKey))
        {
            return new ToolResult
            {
                Success = false,
                Message = "Mailjet credentials not properly configured. Please update the settings with real credentials."
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

            // Send the email using Mailjet
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

    private async Task<bool> SendEmailAsync(
        string to, string subject, string htmlContent, string textContent,
        string cc = null, string bcc = null)
    {
        try
        {
            var emailBuilder = new TransactionalEmailBuilder()
                .WithFrom(new SendContact(_emailSettings.SenderEmail, _emailSettings.SenderName))
                .WithSubject(subject);

            // Add recipients
            foreach (var recipient in ParseAddressesList(to))
                emailBuilder.WithTo(recipient);

            // Add CC
            foreach (var recipient in ParseAddressesList(cc))
                emailBuilder.WithCc(recipient);

            // Add BCC
            foreach (var recipient in ParseAddressesList(bcc))
                emailBuilder.WithBcc(recipient);

            // Set content
            if (!string.IsNullOrEmpty(htmlContent))
                emailBuilder.WithHtmlPart(htmlContent);
            if (!string.IsNullOrEmpty(textContent))
                emailBuilder.WithTextPart(textContent);

            var email = emailBuilder.Build();

            var response = await _mailjetClient.SendTransactionalEmailAsync(email);

            if (response.Messages != null && response.Messages.Length > 0)
            {
                Console.WriteLine("Email sent successfully to {0}", to);
                return true;
            }
            else
            {
                Console.WriteLine("Failed to send email to {0}", to);
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to send email to {0}", to);
            Console.WriteLine("Error: {0}", ex.Message);
            return false;
        }
    }

    private List<SendContact> ParseAddressesList(string addresses)
    {
        var list = new List<SendContact>();
        if (string.IsNullOrEmpty(addresses))
            return list;

        foreach (var address in addresses.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = address.Trim();
            if (!string.IsNullOrEmpty(trimmed))
            {
                list.Add(new SendContact(trimmed));
            }
        }
        return list;
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
        return @"You are an AI assistant that genearte email parameters from natural language instructions.
Your job is to analyze the user's request and extract the necessary information to send an email.

<Rules>
Obey this rules:
- Remain the vibe given in the user's request. Recognize the tone and style of the email based on the user's input.
- Recognize the length that mail should be. If it is status report make it short, if it is a personal message make it longer.
</Rules>

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
}
