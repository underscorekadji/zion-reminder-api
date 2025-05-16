using Zion.Reminder.Models;

namespace Zion.Reminder.Services;

public interface IChannelProcessor
{
    /// <summary>
    /// Determines if the processor can handle notifications for the specified channel
    /// </summary>
    /// <param name="channel">The notification channel to check</param>
    /// <returns>True if the processor can handle the channel, false otherwise</returns>
    bool CanProcessChannel(NotificationChannel channel);

    /// <summary>
    /// Processes a notification asynchronously
    /// </summary>
    /// <param name="notification">The notification to process</param>
    /// <param name="event">The related event</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task ProcessAsync(Notification notification, Event @event);
}
