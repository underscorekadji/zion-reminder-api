using System.ComponentModel.DataAnnotations;

namespace Zion.Reminder.Models;

public class DeleteReviewerEventRequest
{
    [Required]
    [EmailAddress]
    public string FromEmail { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string ToEmail { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string ForEmail { get; set; } = string.Empty;
}
