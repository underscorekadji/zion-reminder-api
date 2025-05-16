using System;
using System.ComponentModel.DataAnnotations;

namespace Zion.Reminder.Models;

public class DeleteReviewerEventRequest : RequestModel
{
    [Required]
    public Person From { get; set; } = new Person();

    [Required]
    public Person To { get; set; } = new Person();

    [Required]
    public Person For { get; set; } = new Person();
      /// <summary>
    /// Validates the request model and throws an exception if validation fails
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    public void Validate()
    {
        // Use the Person class's validation method
        try
        {
            From.Validate();
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"From person validation failed: {ex.Message}", nameof(From));
        }
        
        try
        {
            To.Validate();
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"To person validation failed: {ex.Message}", nameof(To));
        }
        
        try
        {
            For.Validate();
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"For person validation failed: {ex.Message}", nameof(For));
        }
    }
}
