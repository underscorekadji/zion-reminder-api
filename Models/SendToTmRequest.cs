using System;
using System.ComponentModel.DataAnnotations;

namespace Zion.Reminder.Models;

public class SendToTmRequest : RequestModel
{
    [Required]
    public Person TalentManager { get; set; } = new Person();

    [Required]
    public Person Talent { get; set; } = new Person();

    [Required]
    public Person By { get; set; } = new Person(); 
    
    [Url]
    [Required]
    public string? ApplicationLink { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime? EndDate { get; set; }    /// <summary>
    /// Validates the request model and throws an exception if validation fails
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    public void Validate()
    {
        // Validate Person objects
        try { TalentManager.Validate(); } 
        catch (ArgumentException ex) { throw new ArgumentException($"TalentManager validation failed: {ex.Message}", nameof(TalentManager)); }
        
        try { Talent.Validate(); }
        catch (ArgumentException ex) { throw new ArgumentException($"Talent validation failed: {ex.Message}", nameof(Talent)); }
        
        try { By.Validate(); }
        catch (ArgumentException ex) { throw new ArgumentException($"By validation failed: {ex.Message}", nameof(By)); }
        
        // Validate ApplicationLink
        if (string.IsNullOrWhiteSpace(ApplicationLink))
            throw new ArgumentException("Application link is required", nameof(ApplicationLink));
        
        // Convert dates to date-only (without time) for comparison
        var currentDate = DateTime.UtcNow.Date;
        var startDateOnly = StartDate.Date;
        
        // Validate that startDate is today or in the future (ignoring time)
        if (startDateOnly < currentDate)
        {
            throw new ArgumentException("Start date must be today or in the future", nameof(StartDate));
        }

        // Validate endDate if provided (comparing only dates, ignoring time)
        if (EndDate.HasValue && EndDate.Value.Date < startDateOnly)
        {
            throw new ArgumentException("End date must be on or after start date", nameof(EndDate));
        }
    }
}
