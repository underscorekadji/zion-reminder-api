using Microsoft.Extensions.Logging;
using Zion.Reminder.Models;

namespace Zion.Reminder.Services;

public class EmailChannelProcessor : IChannelProcessor
{
    private readonly ILogger<EmailChannelProcessor> _logger;

    public EmailChannelProcessor(ILogger<EmailChannelProcessor> logger)
    {
        _logger = logger;
    }

    public bool CanProcessChannel(NotificationChannel channel)
    {
        return channel == NotificationChannel.Email;
    }

    public Task ProcessAsync(Notification notification, Event @event)
    {
        _logger.LogInformation(
            "Processing Email notification: ID={NotificationId}, To={Address}, Event={EventId}, EventType={EventType}",
            notification.Id,
            notification.ChannelAddress,
            @event.Id,
            @event.Type);

        // In a real implementation, we would send an actual email here
        // For now, we just log the processing

        return Task.CompletedTask;
    }
}
