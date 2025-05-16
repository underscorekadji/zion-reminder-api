using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Zion.Reminder.Config;

namespace Zion.Reminder.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailSettings> emailSettings,
        ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Sends an email to a recipient
    /// </summary>
    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        await SendEmailAsync(to, null, null, subject, body, isHtml);
    }

    /// <summary>
    /// Sends an email with CC and BCC options
    /// </summary>
    public async Task SendEmailAsync(string to, string? cc, string? bcc, string subject, string body, bool isHtml = true)
    {
        try
        {
            // Create the email message
            var message = new MimeMessage();
            
            // Set the sender
            message.From.Add(new MailboxAddress(_emailSettings.FromDisplayName, _emailSettings.FromEmail));
            
            // Add primary recipient
            message.To.Add(MailboxAddress.Parse(to));
            
            // Add CC recipients if provided
            if (!string.IsNullOrEmpty(cc))
            {
                foreach (var recipient in cc.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    message.Cc.Add(MailboxAddress.Parse(recipient.Trim()));
                }
            }
            
            // Add BCC recipients if provided
            if (!string.IsNullOrEmpty(bcc))
            {
                foreach (var recipient in bcc.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    message.Bcc.Add(MailboxAddress.Parse(recipient.Trim()));
                }
            }
            
            // Set subject
            message.Subject = subject;
            
            // Create the message body
            var bodyBuilder = new BodyBuilder();
            if (isHtml)
            {
                bodyBuilder.HtmlBody = body;
            }
            else
            {
                bodyBuilder.TextBody = body;
            }
            
            message.Body = bodyBuilder.ToMessageBody();
            
            // Connect and send the email
            using var client = new SmtpClient();
            
            // Configure the client
            client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            
            // Connect to the SMTP server
            await client.ConnectAsync(
                _emailSettings.Server,
                _emailSettings.Port,
                _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            
            // Authenticate if credentials are provided
            if (!string.IsNullOrEmpty(_emailSettings.Username) && !string.IsNullOrEmpty(_emailSettings.Password))
            {
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            }
            
            // Send the message
            await client.SendAsync(message);
            
            // Disconnect from the server
            await client.DisconnectAsync(true);
            
            _logger.LogInformation("Email sent successfully to {Recipient} with subject '{Subject}'", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient} with subject '{Subject}'", to, subject);
            throw;
        }
    }
}
