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
                "Email (simulated) for notification ID={NotificationId}, To={Address}",
                notification.Id,
                notification.ChannelAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to process email for notification ID={NotificationId}, To={Address}",
                notification.Id,
                notification.ChannelAddress);
            throw;
        }
    }    private string FormatSubject(Event @event)
    {
        string forName = !string.IsNullOrEmpty(@event.ForName) ? @event.ForName : "your colleague";
        
        return @event.Type switch
        {
            EventType.TmNotification => $"Start performance review for {forName}",
            EventType.ReviewerNewNotification => $"Request to provide feedback for {forName}",
            EventType.ReviewerReminderNotification => $"Reminder: Feedback for {forName} is still not submitted",
            _ => $"Zion Reminder: {@event.Type}"
        };
    }
}
