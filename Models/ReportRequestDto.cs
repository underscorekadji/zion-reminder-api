using System.ComponentModel.DataAnnotations;

namespace Zion.Reminder.Models;

public class ReportRequestDto
{
    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; } = string.Empty;
}
