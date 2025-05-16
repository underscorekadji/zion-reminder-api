using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Zion.Reminder.Data;
using Zion.Reminder.Models;

namespace Zion.Reminder.Services;

public class NotificationWorker : BackgroundService
{
    private readonly ILogger<NotificationWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1); // Run every minute

    public NotificationWorker(
        ILogger<NotificationWorker> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NotificationWorker started at: {time}", DateTimeOffset.Now);

        // Run the processing loop while the application is running
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessDueNotifications(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing notifications");
            }

            // Wait for the next cycle
            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task ProcessDueNotifications(CancellationToken stoppingToken)
    {
        // Create a new scope to get scoped services (like DbContext)
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var processor = scope.ServiceProvider.GetRequiredService<INotificationProcessor>();
        
        var now = DateTime.UtcNow;

        // Find all notifications that are ready to be sent
        var notifications = await dbContext.Notifications
            .Include(n => n.Event) // Include related Event data
            .Where(n => n.Status == NotificationStatus.Setupped &&
                        n.SendDateTime <= now)
            .ToListAsync(stoppingToken);

        _logger.LogInformation("Found {Count} notifications to process", notifications.Count);

        // Process each notification
        foreach (var notification in notifications)
        {
            try
            {
                await processor.ProcessNotification(notification);
                
                // Update notification status to Sent (we'd do this in the processor in a real implementation)
                notification.Status = NotificationStatus.Sent;
                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing notification {NotificationId}", notification.Id);
            }
        }
    }
}
