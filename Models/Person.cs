using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Zion.Reminder.Models;

public class Person
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Validates the person has valid email and name values
    /// </summary>
    /// <returns>True if person is valid, otherwise false</returns>
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Name))
            return false;
            
        // Simple regex pattern for email validation
        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(Email, emailPattern);
    }
    
    /// <summary>
    /// Validates the person and throws an exception if not valid
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if person is not valid</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Person name cannot be empty", nameof(Name));
            
        if (string.IsNullOrWhiteSpace(Email))
            throw new ArgumentException("Person email cannot be empty", nameof(Email));
            
        // Simple regex pattern for email validation
        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if (!Regex.IsMatch(Email, emailPattern))
            throw new ArgumentException("Invalid email format", nameof(Email));
    }
}