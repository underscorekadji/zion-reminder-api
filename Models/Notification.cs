using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zion.Reminder.Models;

public class Notification
{
    public int Id { get; set; }

    public int EventId { get; set; }

    [Required]
    public NotificationStatus Status { get; set; } = NotificationStatus.Setupped;

    [Required]
    public NotificationChannel Channel { get; set; }

    [Required]
    [MaxLength(255)]
    public string ChannelAddress { get; set; } = string.Empty;
    // Navigation property - Each notification belongs to one event
    [ForeignKey("EventId")]
    public Event Event { get; set; } = null!;
}

public enum NotificationStatus
{
    Setupped,
    Sent,
    Skipped
}

public enum NotificationChannel
{
    Email,
    Teams
}
