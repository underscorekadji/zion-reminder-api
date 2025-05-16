using System.ComponentModel.DataAnnotations;

namespace Zion.Reminder.Models;

public class SendToBaseRequest : RequestModel
{
    [Required]
    public virtual string ToName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public virtual string ToEmail { get; set; } = string.Empty;

    [Required]
    public virtual string ForName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public virtual string ForEmail { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string From { get; set; } = string.Empty;

    [Required]
    public string FromName { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }
}
