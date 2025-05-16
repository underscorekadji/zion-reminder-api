using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Zion.Reminder.Models;

public class SendToTmRequest : SendToBaseRequest
{
    [Required]
    [JsonPropertyName("TmName")]
    public override string ToName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [JsonPropertyName("TmEmail")]
    public override string ToEmail { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("EmployeeName")]
    public override string ForName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [JsonPropertyName("EmployeeEmail")]
    public override string ForEmail { get; set; } = string.Empty;

    // Link to application
    [Url]
    public string? ApplicationLink { get; set; }

    public DateTime? EndDate { get; set; }
}
