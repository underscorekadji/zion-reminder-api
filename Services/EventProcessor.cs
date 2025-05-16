using Zion.Reminder.Data;
using Zion.Reminder.Models;
using System.Text.Json;

namespace Zion.Reminder.Services;

public interface IEventProcessor
{
    void CreateSendToTmEvent(SendToTmRequest request);
    void CreateSendToReviewerEvent(SendToReviewerRequest request);
}

public class EventProcessor : IEventProcessor
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<EventProcessor> _logger;
    private readonly IConfiguration _configuration;

    public EventProcessor(AppDbContext dbContext, ILogger<EventProcessor> logger, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _logger = logger;
        _configuration = configuration;
    }

    public void CreateSendToReviewerEvent(SendToReviewerRequest request)
    {
        _logger.LogInformation($"Creating reviewer event for {request.ToName} ({request.ToEmail}) with {request.ForEmails.Count} recipients");

        // Basic validation
        if (request.ForNames.Count != request.ForEmails.Count)
            throw new ArgumentException("ForNames and ForEmails must have the same length");
        if (request.ForNames.Count == 0)
            throw new ArgumentException("At least one reviewer must be specified");

        // Get the default Attempt value from config if not provided in the request
        int? configAttempt = null;
        var val = _configuration["Reviewer:DefaultAttempt"];
        if (int.TryParse(val, out int parsedConfigAttempt))
            configAttempt = parsedConfigAttempt;
        int attemptValue = request.Attempt ?? configAttempt ?? throw new InvalidOperationException("Default attempt value is not set in configuration and not provided in request.");

        // Create content object for dynamic data
        var contentData = new
        {
            ApplicationLink = request.ApplicationLink,
            Attempt = attemptValue,
            EndDate = request.EndDate
        };

        var newEvent = new Event
        {
            Type = EventType.ReviewerNotification,
            From = string.Empty, // Not provided in request
            FromName = string.Empty, // Not provided in request
            To = request.ToEmail,
            ToName = request.ToName,
            Status = EventStatus.Open,
            ContentJson = JsonSerializer.Serialize(contentData)
        };

        _dbContext.Events.Add(newEvent);
        _dbContext.SaveChanges(); // To get EventId for notifications

        for (int i = 0; i < request.ForEmails.Count; i++)
        {
            var notification = new Notification
            {
                EventId = newEvent.Id,
                Status = NotificationStatus.Setupped,
                Channel = NotificationChannel.Email,
                ChannelAddress = request.ForEmails[i],
                SendDateTime = DateTime.UtcNow,
                Attempt = i, // Attempt from 0 to attemptValue-1
                NotificationType = i == 0 ? NotificationType.ReviewerNotification : NotificationType.ReminderNotification
            };
            _dbContext.Notifications.Add(notification);
        }

        _dbContext.SaveChanges();

        _logger.LogInformation($"Successfully saved reviewer event with ID {newEvent.Id} and {request.ForEmails.Count} notifications");
    }

    // Removed GetDefaultAttemptFromConfig: logic is now inline in CreateSendToReviewerEvent

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
                SendDateTime = DateTime.UtcNow, // Set send time to now
                NotificationType = NotificationType.ReviewerNotification
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
