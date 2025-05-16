using Microsoft.Extensions.Logging;
using Zion.Reminder.Models;

namespace Zion.Reminder.Services;

public interface INotificationProcessor
{
    Task ProcessNotification(Notification notification);
}

public class NotificationProcessor : INotificationProcessor
{
    private readonly ILogger<NotificationProcessor> _logger;

    public NotificationProcessor(ILogger<NotificationProcessor> logger)
    {
        _logger = logger;
    }

    public Task ProcessNotification(Notification notification)
    {
        // Currently an empty implementation that will be expanded later
        // In future, this is where we'll actually send notifications based on the channel
        
        // Log that we received the notification for processing
        _logger.LogInformation(
            "Processing notification ID: {NotificationId} for Event: {EventId}, Type: {EventType}, Channel: {Channel}",
            notification.Id,
            notification.EventId,
            notification.Event?.Type,
            notification.Channel);

        // Return a completed task for now since we're not doing any real async work
        return Task.CompletedTask;
    }
}
