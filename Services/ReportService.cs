using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zion.Reminder.Data;
using Zion.Reminder.Models;

namespace Zion.Reminder.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _dbContext;
    private readonly IEmailService _emailService;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        AppDbContext dbContext,
        IEmailService emailService,
        ILogger<ReportService> logger)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _logger = logger;
    }    public async Task<bool> GenerateAndSendReportAsync(string emailAddress)
    {
        try
        {
            // Query events with the matching From field and include related notifications
            var events = await _dbContext.Events
                .Where(e => e.From == emailAddress)
                .Include(e => e.Notifications)
                .ToListAsync();

            _logger.LogInformation("Found {Count} events for email address {EmailAddress}", events.Count, emailAddress);            if (events.Count == 0)
            {
                _logger.LogWarning("No events found for email address {EmailAddress}", emailAddress);
                // Don't send an email if no events are found
                return false;
            }            // Serialize to JSON
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true, // Makes the JSON more readable
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve, // Handle circular references
                MaxDepth = 128 // Increase max depth to ensure all data gets serialized
            };
              
            // Create a simplified representation of events for the report
            var reportEvents = events.Select(e => new
            {
                e.Id,
                e.Type,
                e.From,
                e.To,
                e.For,
                e.FromName,
                e.ToName,
                e.ForName,
                e.CreatedAt,
                e.Status,
                e.CorrelationId,
                ContentJson = e.ContentJson,
                Notifications = e.Notifications.Select(n => new
                {
                    n.Id,
                    n.Channel,
                    n.ChannelAddress,
                    n.Status,
                    n.SendDateTime
                }).ToList()
            }).ToList();
              
            var reportData = JsonSerializer.Serialize(reportEvents, jsonOptions);// Send the email with the report
            await SendReportEmailAsync(emailAddress, reportData);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report for email address {EmailAddress}", emailAddress);
            throw;
        }
    }    private async Task SendReportEmailAsync(string emailAddress, string reportData)
    {
        var subject = $"Zion Reminder Report - {DateTime.UtcNow:yyyy-MM-dd}";
        
        // Send the report as a plain text email
        await _emailService.SendEmailAsync(
            emailAddress,
            subject,
            reportData,
            false // Not HTML
        );
        
        _logger.LogInformation("Report email sent to {EmailAddress}", emailAddress);
    }
}
