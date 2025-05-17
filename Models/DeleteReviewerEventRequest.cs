using System;
using System.ComponentModel.DataAnnotations;

namespace Zion.Reminder.Models;

public class DeleteReviewerEventRequest : RequestModel
{
    [Required]
    public Person RequestedBy { get; set; } = new Person();

    [Required]
    public Person Reviewer { get; set; } = new Person();

    [Required]
    public Person Talent { get; set; } = new Person();

    /// <summary>
    /// Validates the request model and throws an exception if validation fails
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    public void Validate()
    {
        // Use the Person class's validation method
        try
        {
            RequestedBy.Validate();
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"RequestedBy person validation failed: {ex.Message}", nameof(RequestedBy));
        }
        
        try
        {
            Reviewer.Validate();
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"Reviewer person validation failed: {ex.Message}", nameof(Reviewer));
        }
        
        try
        {
            Talent.Validate();
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"Talent person validation failed: {ex.Message}", nameof(Talent));
        }
    }
}
