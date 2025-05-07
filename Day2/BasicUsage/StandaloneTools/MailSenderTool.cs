using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using OpenAI.Chat;

namespace Tools;

public class ToolTask
{
    public string ToolName { get; set; } = string.Empty;
    public string Instruction { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();  
}

public class ToolResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, object> ResultData { get; set; } = new Dictionary<string, object>();
}

public class EmailSettings
{
    public string SmtpServer { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public string SenderName { get; set; }
    public string SenderEmail { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    
    public string ApiKey { get; set; } 
}

public class MailSenderTool
{
    private EmailSettings _emailSettings;
    
    public async Task<ToolResult> ExecuteAsync(ToolTask task, EmailSettings settings)
    {
        _emailSettings = settings;
        ChatClient chatClient = new("gpt-4.1",_emailSettings.ApiKey);
        try
        {
            Console.WriteLine("Executing Email Tool with instruction: {0}", task.Instruction);

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
            Console.WriteLine("Error executing Email Tool");
            Console.WriteLine("Error message {0}",ex.Message);
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
    
    private string GetLogicPrompt(string instruction)
    {
        return $"You are a tool that sends emails. Your task is to send an email with the following instruction: {instruction}";
    }
}