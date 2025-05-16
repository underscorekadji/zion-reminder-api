using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zion.Reminder.Models;
using Zion.Reminder.Services;

namespace Zion.Reminder.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportController> _logger;

    public ReportController(
        IReportService reportService,
        ILogger<ReportController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    [HttpPost("GenerateReport")]
    public async Task<IActionResult> GenerateReport([FromBody] ReportRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
          try
        {
            _logger.LogInformation("Generating report for email address: {EmailAddress}", request.EmailAddress);
            
            var reportSent = await _reportService.GenerateAndSendReportAsync(request.EmailAddress);
            
            if (reportSent)
            {
                return Ok();
            }
            else
            {
                return Ok(new { message = "No events found for this email address. No report was sent." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report for email address: {EmailAddress}", request.EmailAddress);
            return StatusCode(500, "An error occurred while generating the report. Please try again later.");
        }
    }
}
