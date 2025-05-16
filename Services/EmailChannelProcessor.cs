using System.Text.Json;
using Microsoft.Extensions.Logging;
using Zion.Reminder.Models;

namespace Zion.Reminder.Services;

public class EmailChannelProcessor : IChannelProcessor
{
    private readonly ILogger<EmailChannelProcessor> _logger;
    private readonly IEmailService _emailService;

    public EmailChannelProcessor(
        ILogger<EmailChannelProcessor> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public bool CanProcessChannel(NotificationChannel channel)
    {
        return channel == NotificationChannel.Email;
    }

    public async Task ProcessAsync(Notification notification, Event @event)
    {
        _logger.LogInformation(
            "Processing Email notification: ID={NotificationId}, To={Address}, Event={EventId}, EventType={EventType}",
            notification.Id,
            notification.ChannelAddress,
            @event.Id,
            @event.Type);

        try
        {
            // Format the subject based on event type
            string subject = FormatSubject(@event);
            
            // Format the email body with event details
            string body = FormatEmailBody(@event);
            
            // Send the email
            await _emailService.SendEmailAsync(
                notification.ChannelAddress,
                subject,
                body,
                true);
            
            // Update notification status to Sent
            notification.Status = NotificationStatus.Sent;
            notification.SendDateTime = DateTime.UtcNow;
            
            _logger.LogInformation(
                "Email sent successfully for notification ID={NotificationId}, To={Address}",
                notification.Id,
                notification.ChannelAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send email for notification ID={NotificationId}, To={Address}",
                notification.Id,
                notification.ChannelAddress);
            
            // We're not updating the status here to allow the worker to retry
            // In a more complex implementation, we could add retry logic or
            // update to a Failed status after X retries
            
            // Re-throw to let the calling code handle the error
            throw;
        }
    }
    
    private string FormatSubject(Event @event)
    {
        return @event.Type switch
        {
            EventType.TmNotification => $"Reminder: Document review required by ",
            EventType.ReviewerNotification => $"Action Required: Please review document by ",
            _ => $"Zion Reminder: Event notification for {@event.Type}"
        };
    }
    
    private string FormatEmailBody(Event @event)
    {
        var eventData = new Dictionary<string, string>
        {
            { "EventType", @event.Type.ToString() },
            { "From", $"{@event.FromName} ({@event.From})" },
            { "To", $"{@event.ToName} ({@event.To})" }
        };
        
        if (!string.IsNullOrEmpty(@event.For))
        {
            eventData.Add("For", $"{@event.ForName} ({@event.For})");
        }
        
        // Try to extract data from ContentJson if available
        if (!string.IsNullOrEmpty(@event.ContentJson))
        {
            try
            {
                var content = JsonSerializer.Deserialize<Dictionary<string, object>>(@event.ContentJson);
                if (content != null)
                {
                    foreach (var item in content)
                    {
                        // Add any additional content data that isn't already in our eventData dictionary
                        if (!eventData.ContainsKey(item.Key))
                        {
                            eventData.Add(item.Key, item.Value?.ToString() ?? string.Empty);
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Could not parse event ContentJson for event ID={EventId}", @event.Id);
            }
        }
        
        // Build HTML email content
        var htmlBuilder = new System.Text.StringBuilder();
        htmlBuilder.AppendLine("<!DOCTYPE html>");
        htmlBuilder.AppendLine("<html>");
        htmlBuilder.AppendLine("<head>");
        htmlBuilder.AppendLine("<style>");
        htmlBuilder.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; }");
        htmlBuilder.AppendLine(".container { max-width: 600px; margin: 0 auto; padding: 20px; }");
        htmlBuilder.AppendLine("table { border-collapse: collapse; width: 100%; }");
        htmlBuilder.AppendLine("table, th, td { border: 1px solid #ddd; }");
        htmlBuilder.AppendLine("th, td { padding: 12px; text-align: left; }");
        htmlBuilder.AppendLine("th { background-color: #f2f2f2; }");
        htmlBuilder.AppendLine("</style>");
        htmlBuilder.AppendLine("</head>");
        htmlBuilder.AppendLine("<body>");
        htmlBuilder.AppendLine("<div class=\"container\">");
        
        // Event type specific content
        switch (@event.Type)
        {
            case EventType.TmNotification:
                htmlBuilder.AppendLine("<h1>Document Review Reminder</h1>");
                htmlBuilder.AppendLine("<p>This is a reminder that you have a document that requires review.</p>");
                break;
            case EventType.ReviewerNotification:
                htmlBuilder.AppendLine("<h1>Document Review Request</h1>");
                htmlBuilder.AppendLine("<p>You have been requested to review a document.</p>");
                break;
            default:
                htmlBuilder.AppendLine($"<h1>Zion Reminder: {@event.Type}</h1>");
                htmlBuilder.AppendLine("<p>You have received a notification from the Zion Reminder system.</p>");
                break;
        }
        
        // Event details table
        htmlBuilder.AppendLine("<h2>Event Details</h2>");
        htmlBuilder.AppendLine("<table>");
        
        foreach (var item in eventData)
        {
            htmlBuilder.AppendLine("<tr>");
            htmlBuilder.AppendLine($"<th>{item.Key}</th>");
            htmlBuilder.AppendLine($"<td>{item.Value}</td>");
            htmlBuilder.AppendLine("</tr>");
        }
        
        htmlBuilder.AppendLine("</table>");
        htmlBuilder.AppendLine("<p>Please take appropriate action before the due date.</p>");
        htmlBuilder.AppendLine("<p>Best regards,<br>Zion Reminder System</p>");
        htmlBuilder.AppendLine("</div>");
        htmlBuilder.AppendLine("</body>");
        htmlBuilder.AppendLine("</html>");
        
        return htmlBuilder.ToString();
    }
    
    private string FormatDate(DateTime? dateTime)
    {
        if (dateTime == null) return "Not specified";
        return dateTime.Value.ToString("MMMM dd, yyyy HH:mm");
    }
}
