namespace Zion.Reminder.Services;

public interface IReportService
{
    /// <summary>
    /// Generates a report for the specified email address and sends it to the same address if events are found
    /// </summary>
    /// <param name="emailAddress">The email address to generate the report for and send it to</param>
    /// <returns>A task representing the asynchronous operation with a boolean indicating if events were found and a report was sent</returns>
    Task<bool> GenerateAndSendReportAsync(string emailAddress);
}
