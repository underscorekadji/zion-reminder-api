using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zion.Reminder.Models;

public class SendToReviewerRequest : RequestModel
{
    [Required]
    public Person RequestedBy { get; set; } = new Person();

    [Required]
    public Person RequestedFor { get; set; } = new Person();

    [Required]
    public List<Person> Reviewers { get; set; } = new();

    public int? Attempt { get; set; }

    [Url]
    public string? ApplicationLink { get; set; }

    public DateTime? EndDate { get; set; }    /// <summary>
    /// Validates the request model and throws an exception if validation fails
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    public void Validate()
    {
        // Validate Person objects
        try { RequestedBy.Validate(); } 
        catch (ArgumentException ex) { throw new ArgumentException($"RequestedBy validation failed: {ex.Message}", nameof(RequestedBy)); }
        
        try { RequestedFor.Validate(); }
        catch (ArgumentException ex) { throw new ArgumentException($"RequestedFor validation failed: {ex.Message}", nameof(RequestedFor)); }
        
        // Validate reviewers list
        if (Reviewers == null || Reviewers.Count == 0)
            throw new ArgumentException("At least one reviewer must be specified", nameof(Reviewers));
            
        // Validate each reviewer
        for (int i = 0; i < Reviewers.Count; i++)
        {
            try { Reviewers[i].Validate(); }
            catch (ArgumentException ex) { throw new ArgumentException($"Reviewer at index {i} validation failed: {ex.Message}", nameof(Reviewers)); }
        }
        
        // Validate end date
        if (!EndDate.HasValue)
            throw new ArgumentException("EndDate must be provided for scheduling notifications", nameof(EndDate));
            
        // Compare only dates, ignoring time component
        var currentDate = DateTime.UtcNow.Date;
        if (EndDate.Value.Date <= currentDate)
            throw new ArgumentException("EndDate must be a future date", nameof(EndDate));
    }
}
