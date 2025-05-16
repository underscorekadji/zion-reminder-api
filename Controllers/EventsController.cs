using Microsoft.AspNetCore.Mvc;
using Zion.Reminder.Data;
using Zion.Reminder.Models;

namespace Zion.Reminder.Controllers;

using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<EventsController> _logger;

    public EventsController(AppDbContext context, ILogger<EventsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/events/sent-to-tm
    [HttpGet("sent-to-tm")]
    public IActionResult GetSentToTm()
    {
        // Empty response as requested
        return Ok();
    }
}
