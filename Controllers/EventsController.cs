using Microsoft.AspNetCore.Mvc;
using Zion.Reminder.Models;
using Zion.Reminder.Services;

namespace Zion.Reminder.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly ILogger<EventsController> _logger;
    private readonly IEventProcessor _eventProcessor;

    public EventsController(ILogger<EventsController> logger, IEventProcessor eventProcessor)
    {
        _logger = logger;
        _eventProcessor = eventProcessor;
    }    // POST: api/events/send-to-tm
    [HttpPost("send-to-tm")]
    public IActionResult SendToTm([FromBody] SendToTmRequest request)
    {
        _logger.LogInformation($"Received request to send notification to TM {request.TmEmail}");
        
        _eventProcessor.CreateSendToTmEvent(
            request.TmName,
            request.TmEmail,
            request.EmployeeName,
            request.EmployeeEmail,
            request.From,
            request.FromName,
            request.StartDate,
            request.CorrelationId,
            request.ApplicationLink);

        return Ok(new { success = true, message = "Event created successfully" });
    }
}
