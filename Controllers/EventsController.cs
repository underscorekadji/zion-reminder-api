using Microsoft.AspNetCore.Mvc;
using Zion.Reminder.Models;
using Zion.Reminder.Services;
using Microsoft.AspNetCore.Authorization;

namespace Zion.Reminder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly IEventProcessor _eventProcessor;

        public EventsController(ILogger<EventsController> logger, IEventProcessor eventProcessor)
        {
            _logger = logger;
            _eventProcessor = eventProcessor;
        }

        [HttpPost("send-to-tm")]
        public IActionResult SendToTm([FromBody] SendToTmRequest request)
        {
            _logger.LogInformation("Received request to send notification to TM {ToEmail}", request.ToEmail);

            _eventProcessor.CreateSendToTmEvent(request);

            return Ok(new { success = true, message = "Event created successfully" });
        }
    }
}
