using Zion.Reminder.Data;
using Zion.Reminder.Models;
using System.Text.Json;

namespace Zion.Reminder.Services;

public interface IEventProcessor
{
    void CreateSendToTmEvent(SendToTmRequest request);
}

public class EventProcessor : IEventProcessor
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<EventProcessor> _logger;

    public EventProcessor(AppDbContext dbContext, ILogger<EventProcessor> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public void CreateSendToTmEvent(SendToTmRequest request)
    {
        _logger.LogInformation($"Creating event for TM {request.ToName} ({request.ToEmail}) regarding employee {request.ForName} ({request.ForEmail})");
        _logger.LogInformation($"From: {request.FromName} ({request.From}), Start Date: {request.StartDate}, End Date: {request.EndDate}, Correlation ID: {request.CorrelationId}");
        // Validate that startDate is now or in the future
        if (request.StartDate < DateTime.UtcNow)
        {
            _logger.LogWarning($"Invalid start date: {request.StartDate}. Start date must be now or in the future.");
            throw new ArgumentException("Start date must be now or in the future.", nameof(request.StartDate));
        }

        // Validate endDate if provided
        if (request.EndDate.HasValue && request.EndDate.Value < request.StartDate)
        {
            _logger.LogWarning($"Invalid end date: {request.EndDate}. End date must be after start date: {request.StartDate}.");
            throw new ArgumentException("End date must be after start date.", nameof(request.EndDate));
        }

        try
        {
            // Create content object for dynamic data including startDate and endDate
            var contentData = new
            {
                ApplicationLink = request.ApplicationLink,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };
            var newEvent = new Event
            {
                Type = EventType.TmNotification,
                From = request.From,
                FromName = request.FromName,
                To = request.ToEmail,
                ToName = request.ToName,
                For = request.ForEmail,
                ForName = request.ForName,
                Status = EventStatus.Open,
                CorrelationId = request.CorrelationId,
                ContentJson = JsonSerializer.Serialize(contentData)
            };

            _dbContext.Events.Add(newEvent);

            // Create notification with current time as send time
            var notification = new Notification
            {
                EventId = 0, // Will be set correctly after SaveChanges
                Status = NotificationStatus.Setupped,
                Channel = NotificationChannel.Email,
                ChannelAddress = request.ToEmail,
                SendDateTime = DateTime.UtcNow // Set send time to now
            };

            // Add notification to event
            newEvent.Notifications.Add(notification);

            _dbContext.SaveChanges();

            _logger.LogInformation($"Successfully saved event with ID {newEvent.Id} to database");
            _logger.LogInformation($"Created notification with ID {notification.Id} set to send at {notification.SendDateTime}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating and saving event");
            throw;
        }
    }
}
