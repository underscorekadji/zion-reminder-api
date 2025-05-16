using System.Text.Json;
using Microsoft.Extensions.Logging;
using Zion.Reminder.Models;

namespace Zion.Reminder.Services;

public class EmailChannelProcessor : IChannelProcessor
{
    private readonly ILogger<EmailChannelProcessor> _logger;
    private readonly IEmailService _emailService;
    private readonly IMessageGenerator _messageGenerator;

    public EmailChannelProcessor(
        ILogger<EmailChannelProcessor> logger,
        IEmailService emailService,
        IMessageGenerator messageGenerator)
    {
        _logger = logger;
        _emailService = emailService;
        _messageGenerator = messageGenerator;
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

            // Use IMessageGenerator to create the email body
            string body = await _messageGenerator.GenerateMessageBodyAsync(@event, notification);

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
            EventType.ReviewerNewNotification => $"Action Required: Please review document by ",
            _ => $"Zion Reminder: Event notification for {@event.Type}"
        };
    }

    private string FormatDate(DateTime? dateTime)
    {
        if (dateTime == null) return "Not specified";
        return dateTime.Value.ToString("MMMM dd, yyyy HH:mm");
    }
}
