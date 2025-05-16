namespace Zion.Reminder.Models;

public class Reminder
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
}

public enum Priority
{
    Low,
    Medium,
    High,
    Urgent
}
