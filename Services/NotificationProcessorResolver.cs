using Microsoft.Extensions.Logging;
using Zion.Reminder.Models;

namespace Zion.Reminder.Services;

public interface INotificationProcessorResolver
{
    /// <summary>
    /// Gets the appropriate channel processor for the specified notification
    /// </summary>
    /// <param name="notification">The notification to find a processor for</param>
    /// <returns>The appropriate channel processor</returns>
    /// <exception cref="NotSupportedException">Thrown when no processor supports the notification channel</exception>
    IChannelProcessor GetProcessorForNotification(Notification notification);
}

public class NotificationProcessorResolver : INotificationProcessorResolver
{
    private readonly IEnumerable<IChannelProcessor> _processors;
    private readonly ILogger<NotificationProcessorResolver> _logger;

    public NotificationProcessorResolver(
        IEnumerable<IChannelProcessor> processors,
        ILogger<NotificationProcessorResolver> logger)
    {
        _processors = processors;
        _logger = logger;
    }

    public IChannelProcessor GetProcessorForNotification(Notification notification)
    {
        _logger.LogDebug("Finding processor for notification channel: {Channel}", notification.Channel);
        
        // Find the appropriate processor for this notification channel
        var processor = _processors.FirstOrDefault(p => p.CanProcessChannel(notification.Channel));
        
        if (processor == null)
        {
            _logger.LogError("No processor found for notification channel: {Channel}", notification.Channel);
            throw new NotSupportedException($"No processor found for notification channel: {notification.Channel}");
        }
        
        _logger.LogDebug("Found processor of type {ProcessorType} for channel {Channel}",
            processor.GetType().Name,
            notification.Channel);
            
        return processor;
    }
}
