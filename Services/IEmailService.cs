namespace Zion.Reminder.Services;

public interface IEmailService
{
    /// <summary>
    /// Sends an email with the specified parameters
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body</param>
    /// <param name="isHtml">Whether the body contains HTML content</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    
    /// <summary>
    /// Sends an email with the specified parameters
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="cc">Carbon copy recipients (optional)</param>
    /// <param name="bcc">Blind carbon copy recipients (optional)</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body</param>
    /// <param name="isHtml">Whether the body contains HTML content</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendEmailAsync(string to, string? cc, string? bcc, string subject, string body, bool isHtml = true);
}
