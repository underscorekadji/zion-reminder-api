using System.ComponentModel.DataAnnotations;

namespace Zion.Reminder.Models;

public class SendToTmRequest : RequestModel
{
    [Required]
    public string TmName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string TmEmail { get; set; } = string.Empty;
    
    [Required]
    public string EmployeeName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string EmployeeEmail { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string From { get; set; } = string.Empty;
    
    [Required]
    public string FromName { get; set; } = string.Empty;
    
    [Required]
    public DateTime StartDate { get; set; }
    
    // Link to application
    [Url]
    public string? ApplicationLink { get; set; }
}
