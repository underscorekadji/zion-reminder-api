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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Received request to send notification to TM {ToEmail}", request.TalentManager.Email);

            // The request model's Validate method will be called by the EventProcessor
            _eventProcessor.CreateSendToTmEvent(request);

            return Ok(new { success = true, message = "Event created successfully" });
        }

        [HttpDelete("send-to-reviewer")]
        public IActionResult DeleteReviewerNotifications([FromBody] DeleteReviewerEventRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            request.Validate();
            _eventProcessor.DeleteReviewerNotifications(request);
            return Ok(new { success = true, message = "Reviewer event closed and notifications skipped if not sent." });
        }

        [HttpPost("send-to-reviewer")]
        public IActionResult SendToReviewer([FromBody] SendToReviewerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Received request to send to reviewer: RequestedBy: {RequestedByEmail}, RequestedFor: {RequestedForEmail}, Reviewers count: {ReviewersCount}",
                request.RequestedBy.Email, request.RequestedFor.Email, request.Reviewers.Count);

            // The request model's Validate method will be called by the EventProcessor
            _eventProcessor.CreateSendToReviewerEvent(request);

            return Ok(new { success = true, message = "Reviewer event and notifications created successfully" });
        }
    }
}
