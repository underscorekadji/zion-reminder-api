using Zion.Reminder.Data;
using Zion.Reminder.Models;
using Zion.Reminder.Config;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Zion.Reminder.Services;

public interface IEventProcessor
{
    void CreateSendToTmEvent(SendToTmRequest request);
    void DeleteReviewerNotifications(DeleteReviewerEventRequest request);
    void CreateSendToReviewerEvent(SendToReviewerRequest request);
}

public class EventProcessor : IEventProcessor
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<EventProcessor> _logger;
    private readonly IConfiguration _configuration;
    private readonly ReviewerSettings _reviewerSettings;

    public EventProcessor(
        AppDbContext dbContext,
        ILogger<EventProcessor> logger,
        IConfiguration configuration,
        IOptions<ReviewerSettings> reviewerSettings)
    {
        _dbContext = dbContext;
        _logger = logger;
        _configuration = configuration;
        _reviewerSettings = reviewerSettings.Value;
    }
    public void CreateSendToReviewerEvent(SendToReviewerRequest request)
    {
        _logger.LogInformation($"Creating reviewer events for {request.RequestedBy.Name} ({request.RequestedBy.Email}) regarding {request.RequestedFor.Name} with {request.Reviewers.Count} reviewers");

        // Validate the request model
        request.Validate();

        // Get the configured attempt value
        int attemptValue = GetConfiguredAttempts(request.Attempt);

        // Create content object for dynamic data
        var contentData = new
        {
            request.ApplicationLink,
            Attempt = attemptValue,
            request.EndDate
        };

        var serializedContent = JsonSerializer.Serialize(contentData);        // Calculate reminder schedule if we have multiple attempts
        var reminderSchedule = new List<DateTime>();
        if (attemptValue > 0 && request.EndDate.HasValue)
        {
            reminderSchedule = GetNotificationSchedule(DateTime.UtcNow, request.EndDate.Value, attemptValue);

            // Log if the number of reminders is less than requested attempts due to day limitations
            if (reminderSchedule.Count < attemptValue)
            {
                _logger.LogWarning($"Requested {attemptValue} reminder attempts but only created {reminderSchedule.Count} due to day limitations");
            }
        }

        int totalNotifications = 0;

        // Create events and notifications for each reviewer
        foreach (var reviewer in request.Reviewers)
        {
            // Create New Notification Event
            var newNotificationEvent = new Event
            {
                Type = EventType.ReviewerNewNotification,
                From = request.RequestedBy.Email,
                FromName = request.RequestedBy.Name,
                To = reviewer.Email,
                ToName = reviewer.Name,
                For = request.RequestedFor.Email,
                ForName = request.RequestedFor.Name,
                Status = EventStatus.Open,
                ContentJson = serializedContent
            };

            _dbContext.Events.Add(newNotificationEvent);
            _dbContext.SaveChanges(); // To get EventId for notifications

            // Create one immediate notification for the new notification event
            var newNotification = new Notification
            {
                EventId = newNotificationEvent.Id,
                Status = NotificationStatus.Setupped,
                Channel = NotificationChannel.Email,
                ChannelAddress = reviewer.Email,
                SendDateTime = DateTime.UtcNow, // Send immediately
                Attempt = 0,
                NotificationType = NotificationType.ReviewerNotification
            };

            _dbContext.Notifications.Add(newNotification);
            totalNotifications++;

            // Only create reminder event and notifications if we have reminders to send
            if (reminderSchedule.Count > 0)
            {
                // Create Reminder Notification Event
                var reminderEvent = new Event
                {
                    Type = EventType.ReviewerReminderNotification,
                    From = request.RequestedBy.Email,
                    FromName = request.RequestedBy.Name,
                    To = reviewer.Email,
                    ToName = reviewer.Name,
                    For = request.RequestedFor.Email,
                    ForName = request.RequestedFor.Name,
                    Status = EventStatus.Open,
                    ContentJson = serializedContent
                };

                _dbContext.Events.Add(reminderEvent);
                _dbContext.SaveChanges(); // To get EventId for notifications

                // Create multiple reminder notifications with scheduled times
                for (int j = 0; j < reminderSchedule.Count; j++)
                {
                    var reminderNotification = new Notification
                    {
                        EventId = reminderEvent.Id,
                        Status = NotificationStatus.Setupped,
                        Channel = NotificationChannel.Email,
                        ChannelAddress = reviewer.Email,
                        SendDateTime = reminderSchedule[j],
                        Attempt = j + 1, // Attempt 1, 2, 3, etc.
                        NotificationType = NotificationType.ReminderNotification
                    };

                    _dbContext.Notifications.Add(reminderNotification);
                    totalNotifications++;
                }
            }
        }

        _dbContext.SaveChanges();

        _logger.LogInformation($"Successfully created events and {totalNotifications} notifications for {request.Reviewers.Count} reviewers");
    }
    public static List<DateTime> GetNotificationSchedule(DateTime startDate, DateTime endDate, int attempts)
    {
        if (attempts <= 0)
            throw new ArgumentException("Number of attempts must be greater than 0", nameof(attempts));

        // Make sure we're working with dates at the start of the day
        var startDay = startDate.Date;
        var endDay = endDate.Date;

        if (endDay <= startDay)
            throw new ArgumentException("End date must be after start date");

        var result = new List<DateTime>();

        // Calculate available days (excluding start day, but including end day)
        int availableDays = (int)(endDay - startDay).TotalDays;

        // If we have fewer days than requested attempts, limit attempts to available days
        int actualAttempts = Math.Min(attempts, availableDays);

        if (actualAttempts == 0)
        {
            // If no days available for attempts, return empty list
            return result;
        }

        // Always ensure the last notification is sent on the end date (deadline day)
        if (actualAttempts == 1)
        {
            // If only one attempt, it should be on the deadline day
            var sendTime = endDay.AddHours(10); // 10 AM on deadline day for urgent attention
            result.Add(sendTime);
        }
        else
        {
            // Last notification is always on the deadline day
            result.Add(endDay.AddHours(10)); // 10 AM on deadline day

            // If we have more than one attempt, distribute the rest evenly from start to one day before end
            if (actualAttempts > 1)
            {
                // Calculate days between start and one day before end date
                int daysUntilOneBeforeEnd = availableDays - 1;

                // Number of attempts excluding the final one
                int remainingAttempts = actualAttempts - 1;

                // If we have days for remaining attempts
                if (daysUntilOneBeforeEnd > 0 && remainingAttempts > 0)
                {
                    double step = (double)daysUntilOneBeforeEnd / (remainingAttempts + 1);

                    for (int i = 1; i <= remainingAttempts; i++)
                    {
                        // Calculate the day number for this attempt
                        int dayNumber = (int)Math.Round(step * i);

                        // Create notification at noon on that day
                        var sendTime = startDay.AddDays(dayNumber).AddHours(12);
                        result.Add(sendTime);
                    }
                }
            }

            // Ensure no duplicate days (round-off errors might cause) and sort by date
            result = result.GroupBy(x => x.Date)
                          .Select(g => g.First())
                          .OrderBy(d => d)
                          .ToList();
        }

        return result;
    }
    public void CreateSendToTmEvent(SendToTmRequest request)
    {
        _logger.LogInformation($"Creating event for TM {request.TalentManager.Name} ({request.TalentManager.Email}) regarding employee {request.Talent.Name} ({request.Talent.Email})");
        _logger.LogInformation($"From: {request.By.Name} ({request.By.Email}), Start Date: {request.StartDate}, End Date: {request.EndDate}, Correlation ID: {request.CorrelationId}");

        // Validate the request model
        request.Validate();

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
                From = request.By.Email,
                FromName = request.By.Name,
                To = request.TalentManager.Email,
                ToName = request.TalentManager.Name,
                For = request.Talent.Email,
                ForName = request.Talent.Name,
                Status = EventStatus.Open,
                CorrelationId = request.CorrelationId,
                ContentJson = JsonSerializer.Serialize(contentData)
            };

            _dbContext.Events.Add(newEvent);            // Create notification with current time as send time
            var notification = new Notification
            {
                EventId = 0, // Will be set correctly after SaveChanges
                Status = NotificationStatus.Setupped,
                Channel = NotificationChannel.Email,
                ChannelAddress = request.TalentManager.Email,
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
    public void DeleteReviewerNotifications(DeleteReviewerEventRequest request)
    {
        // Validate the request model
        request.Validate();

        var openEvent = _dbContext.Events
            .Where(e => (e.Type == EventType.ReviewerNewNotification || e.Type == EventType.ReviewerReminderNotification)
                        && e.Status == EventStatus.Open
                        && e.From == request.From.Email
                        && e.To == request.To.Email
                        && e.For == request.For.Email)
            .FirstOrDefault(); if (openEvent == null)
        {
            var message = $"No open reviewer event found for from={request.From.Email}, to={request.To.Email}, for={request.For.Email}";
            _logger.LogWarning(message);
            throw new ArgumentException(message);
        }

        var notifications = _dbContext.Notifications
            .Where(n => n.EventId == openEvent.Id && n.Status == NotificationStatus.Setupped)
            .ToList();

        foreach (var notification in notifications)
        {
            notification.Status = NotificationStatus.Skipped;
        }

        openEvent.Status = EventStatus.Closed;
        _dbContext.SaveChanges();
        _logger.LogInformation("Reviewer event {EventId} closed and notifications skipped if not sent.", openEvent.Id);
    }      /// <summary>
           /// Gets the configured attempt value from request or default settings.
           /// The actual number of attempts will be limited by the number of days available between start and end dates.
           /// </summary>
           /// <param name="requestAttempt">Optional attempt value from the request</param>
           /// <returns>The configured attempt value</returns>
    private int GetConfiguredAttempts(int? requestAttempt)
    {
        // Use the provided attempt value or fall back to the default from settings
        int attemptValue = requestAttempt ?? _reviewerSettings.DefaultAttempt;

        // Ensure we have at least 1 attempt
        if (attemptValue <= 0)
        {
            _logger.LogWarning($"Invalid attempt value {attemptValue}. Using default value of 1.");
            attemptValue = 1;
        }

        return attemptValue;
    }
}
