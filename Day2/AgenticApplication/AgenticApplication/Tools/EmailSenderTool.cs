using AgenticApplication.Data.Models;
using AgenticApplication.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AgenticApplication.Tools;

public class EmailSenderTool : ITool
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailSenderTool> _logger;

    public EmailSenderTool(
        IOptions<EmailSettings> emailSettings,
        ILogger<EmailSenderTool> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public string ToolName => "email-sender";

    public string GetDescription()
    {
        return "Sends emails to recipients. Use this tool to compose and send emails with customized content. " +
               "Can handle HTML formatting, subject lines, and multiple recipients including CC and BCC."+
               "if email address is not known then to field should be empty. ";
    }

    public async Task<ToolResult> ExecuteAsync(ToolTask task)
    {
        try
        {
            _logger.LogInformation("Executing Email Tool with instruction: {Instruction}", task.Instruction);

            // Extract email parameters from the task data
            if (!TryExtractEmailData(task.Data, out var to, out var subject, out var htmlContent, 
                    out var textContent, out var cc, out var bcc))
            {
                return new ToolResult
                {
                    Success = false,
                    Message = "Missing required email parameters (to, subject, or content)"
                };
            }

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
            _logger.LogError(ex, "Error executing Email Tool");
            return new ToolResult
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }

    private bool TryExtractEmailData(Dictionary<string, object> data, 
        out string to, out string subject, out string htmlContent, 
        out string textContent, out string cc, out string bcc)
    {
        to = string.Empty;
        subject = string.Empty;
        htmlContent = string.Empty;
        textContent = string.Empty;
        cc = string.Empty;
        bcc = string.Empty;

        if (data == null)
            return false;

        // Extract required fields
        if (!data.TryGetValue("to", out var toObj) || toObj == null)
            return false;
        to = toObj.ToString();

        if (!data.TryGetValue("subject", out var subjectObj) || subjectObj == null)
            return false;
        subject = subjectObj.ToString();

        if (data.TryGetValue("htmlContent", out var htmlContentObj) && htmlContentObj != null)
            htmlContent = htmlContentObj.ToString();

        if (data.TryGetValue("textContent", out var textContentObj) && textContentObj != null)
            textContent = textContentObj.ToString();

        // If neither HTML nor text content is provided, return false
        if (string.IsNullOrEmpty(htmlContent) && string.IsNullOrEmpty(textContent))
            return false;

        // Extract optional fields
        if (data.TryGetValue("cc", out var ccObj) && ccObj != null)
            cc = ccObj.ToString();

        if (data.TryGetValue("bcc", out var bccObj) && bccObj != null)
            bcc = bccObj.ToString();

        return true;
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
                
            _logger.LogInformation("Email sent successfully to {Recipient}", to);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient}", to);
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