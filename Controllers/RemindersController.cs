using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zion.Reminder.Data;
using Zion.Reminder.Models;

namespace Zion.Reminder.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RemindersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<RemindersController> _logger;

    public RemindersController(AppDbContext context, ILogger<RemindersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/reminders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Reminder>>> GetReminders()
    {
        return await _context.Reminders.ToListAsync();
    }

    // GET: api/reminders/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Reminder>> GetReminder(int id)
    {
        var reminder = await _context.Reminders.FindAsync(id);

        if (reminder == null)
        {
            return NotFound();
        }

        return reminder;
    }

    // POST: api/reminders
    [HttpPost]
    public async Task<ActionResult<Reminder>> CreateReminder(Reminder reminder)
    {
        reminder.CreatedAt = DateTime.UtcNow;
        _context.Reminders.Add(reminder);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReminder), new { id = reminder.Id }, reminder);
    }

    // PUT: api/reminders/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReminder(int id, Reminder reminder)
    {
        if (id != reminder.Id)
        {
            return BadRequest();
        }

        _context.Entry(reminder).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ReminderExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // PATCH: api/reminders/5/complete
    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> CompleteReminder(int id)
    {
        var reminder = await _context.Reminders.FindAsync(id);
        
        if (reminder == null)
        {
            return NotFound();
        }

        reminder.IsCompleted = true;
        reminder.CompletedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    // DELETE: api/reminders/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReminder(int id)
    {
        var reminder = await _context.Reminders.FindAsync(id);
        if (reminder == null)
        {
            return NotFound();
        }

        _context.Reminders.Remove(reminder);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ReminderExists(int id)
    {
        return _context.Reminders.Any(e => e.Id == id);
    }
}
