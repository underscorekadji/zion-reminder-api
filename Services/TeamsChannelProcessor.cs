using Microsoft.Extensions.Logging;
using Zion.Reminder.Models;

namespace Zion.Reminder.Services;

public class TeamsChannelProcessor : IChannelProcessor
{
    private readonly ILogger<TeamsChannelProcessor> _logger;

    public TeamsChannelProcessor(ILogger<TeamsChannelProcessor> logger)
    {
        _logger = logger;
    }

    public bool CanProcessChannel(NotificationChannel channel)
    {
        return channel == NotificationChannel.Teams;
    }

    public Task ProcessAsync(Notification notification, Event @event)
    {
        _logger.LogInformation(
            "Processing Teams notification: ID={NotificationId}, Webhook={Address}, Event={EventId}, EventType={EventType}",
            notification.Id,
            notification.ChannelAddress,
            @event.Id,
            @event.Type);

        // In a real implementation, we would send an actual Teams message here
        // For now, we just log the processing

        return Task.CompletedTask;
    }
}
