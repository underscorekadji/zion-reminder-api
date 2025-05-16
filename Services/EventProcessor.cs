using Zion.Reminder.Data;
using Zion.Reminder.Models;
using System.Text.Json;

namespace Zion.Reminder.Services;

public interface IEventProcessor
{
    void CreateSendToTmEvent(string tmName, string tmEmail, string employeeName, string employeeEmail, DateTime startDate, Guid correlationId, string? applicationLink = null);
}

public class EventProcessor : IEventProcessor
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<EventProcessor> _logger;

    public EventProcessor(AppDbContext dbContext, ILogger<EventProcessor> logger)
    {
        _dbContext = dbContext;
        _logger = logger;    }
    
    public void CreateSendToTmEvent(string tmName, string tmEmail, string employeeName, string employeeEmail, DateTime startDate, Guid correlationId, string? applicationLink = null)
    {
        _logger.LogInformation($"Creating event for TM {tmName} ({tmEmail}) regarding employee {employeeName} ({employeeEmail})");
        _logger.LogInformation($"Start Date: {startDate}, Correlation ID: {correlationId}");
          try
        {
            // Create content object for dynamic data
            var contentData = new 
            {
                ApplicationLink = applicationLink
            };
              var newEvent = new Event
            {
                Type = EventType.TmNotification,
                From = "system@zion-reminder.com",  // System generated
                FromName = "Zion Reminder System",
                To = tmEmail,
                ToName = tmName,
                For = employeeEmail,
                ForName = employeeName,
                StartDate = startDate,
                Status = EventStatus.Open,
                CorrelationId = correlationId,
                ContentJson = JsonSerializer.Serialize(contentData)
            };
            
            _dbContext.Events.Add(newEvent);
            _dbContext.SaveChanges();
            
            _logger.LogInformation($"Successfully saved event with ID {newEvent.Id} to database");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating and saving event");
            throw;
        }
    }
}
