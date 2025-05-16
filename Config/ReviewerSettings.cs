using System;

namespace Zion.Reminder.Config;

/// <summary>
/// Configuration settings for reviewer notifications.
/// 
/// Values can be provided through multiple sources (in order of precedence):
/// 1. Environment variables
/// 2. User Secrets (in Development)
/// 3. appsettings.{environment}.json
/// 4. appsettings.json - lowest priority
/// </summary>
public class ReviewerSettings
{
    /// <summary>
    /// Default number of reminder attempts to send to reviewers if not specified in the request.
    /// The actual number of attempts may be limited by the number of days available.
    /// </summary>
    public int DefaultAttempt { get; set; } = 3;
}
