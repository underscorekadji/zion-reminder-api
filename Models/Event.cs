using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zion.Reminder.Models;

public class Event
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public EventType Type { get; set; }

    [Required]
    [MaxLength(255)]
    public string From { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string To { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? For { get; set; }

    [MaxLength(255)]
    public string FromName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string ToName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? ForName { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    
    [Required] 
    public EventStatus Status { get; set; } = EventStatus.Open;
    
    // Correlation ID for tracking related operations across services
    public Guid? CorrelationId { get; set; }
    
    // Field for storing dynamic data in JSON format
    public string? ContentJson { get; set; }

    // Navigation property - One event has many notifications
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}

public enum EventType
{
    TmNotification,
    ReviewerNotification
}

public enum EventStatus
{
    Open,
    Closed
}
