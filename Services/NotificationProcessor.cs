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
    private readonly INotificationProcessorResolver _processorResolver;

    public NotificationProcessor(
        ILogger<NotificationProcessor> logger,
        INotificationProcessorResolver processorResolver)
    {
        _logger = logger;
        _processorResolver = processorResolver;
    }

    public async Task ProcessNotification(Notification notification)
    {
        _logger.LogInformation(
            "Processing notification ID: {NotificationId} for Event: {EventId}, Type: {EventType}, Channel: {Channel}",
            notification.Id,
            notification.EventId,
            notification.Event?.Type,
            notification.Channel);

        try
        {
            // Get the appropriate processor for this notification channel
            var channelProcessor = _processorResolver.GetProcessorForNotification(notification);

            // Null check for notification.Event
            if (notification.Event == null)
            {
                _logger.LogWarning(
                    "Notification ID: {NotificationId} has a null Event. Skipping processing.",
                    notification.Id);
                return;
            }

            // Process the notification using the selected processor
            await channelProcessor.ProcessAsync(notification, notification.Event);

            _logger.LogInformation(
                "Successfully processed notification ID: {NotificationId} using {ProcessorType}",
                notification.Id, 
                channelProcessor.GetType().Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing notification ID: {NotificationId}, Channel: {Channel}",
                notification.Id,
                notification.Channel);

            // Re-throw to allow the worker to handle the error appropriately
            throw;
        }
    }
}
