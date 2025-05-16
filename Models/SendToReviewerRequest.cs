using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Zion.Reminder.Models;

public class SendToReviewerRequest
{
    [Required]
    [JsonPropertyName("ToName")]
    public string ToName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [JsonPropertyName("ToEmail")]
    public string ToEmail { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("ForNames")]
    public List<string> ForNames { get; set; } = new();

    [Required]
    [JsonPropertyName("ForEmails")]
    public List<string> ForEmails { get; set; } = new();

    [JsonPropertyName("Attempt")]
    public int? Attempt { get; set; }


    [Url]
    public string? ApplicationLink { get; set; }

    public DateTime? EndDate { get; set; }
}
