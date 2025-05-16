namespace Zion.Reminder.Config;

/// <summary>
/// Email configuration settings for SMTP email service.
/// 
/// Values can be provided through multiple sources (in order of precedence):
/// 1. Environment variables (e.g., EMAIL_PASSWORD) - highest priority
/// 2. User Secrets (in Development) 
/// 3. appsettings.{environment}.json
/// 4. appsettings.json - lowest priority
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// SMTP server address (e.g., smtp.gmail.com)
    /// </summary>
    public string Server { get; set; } = string.Empty;
    
    /// <summary>
    /// SMTP server port (default: 587 for TLS)
    /// </summary>
    public int Port { get; set; } = 587;
    
    /// <summary>
    /// SMTP authentication username
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// SMTP authentication password
    /// Can be provided via:
    /// - Environment variable: EMAIL_PASSWORD
    /// - User Secrets: "EmailSettings:Password"
    /// - Configuration file (not recommended for production)
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Email address used in the From field
    /// </summary>
    public string FromEmail { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name used in the From field
    /// </summary>
    public string FromDisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether to use SSL/TLS for the SMTP connection
    /// </summary>
    public bool EnableSsl { get; set; } = true;
}
