using System.ComponentModel.DataAnnotations;

namespace Zion.Reminder.Models;

/// <summary>
/// Base class for all request models
/// </summary>
public abstract class RequestModel
{
    /// <summary>
    /// Unique identifier for correlating requests across services
    /// </summary>
    [Required]
    public Guid CorrelationId { get; set; }
}
