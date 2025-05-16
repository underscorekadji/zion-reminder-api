using System.Threading.Tasks;
using Zion.Reminder.Models;
using OpenAI.GPT3;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using Microsoft.Extensions.Configuration;

namespace Zion.Reminder.Services;

public class MessageGenerator : IMessageGenerator
{
    private readonly IOpenAIService _openAiService;

    public MessageGenerator(IOpenAIService openAiService)
    {
        _openAiService = openAiService;
    }

    public async Task<string> GenerateMessageBodyAsync(Event @event, Notification notification)
    {
        var prompt = PromptFactory(@event, notification);

        var completionResult = await _openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem(prompt)
            },
            Model = "gpt-3.5-turbo"
        });

        if (completionResult.Successful && completionResult.Choices.Count > 0)
        {
            return completionResult.Choices[0].Message.Content.Trim();
        }
        else
        {
            throw new Exception($"OpenAI API call failed: {completionResult.Error?.Message}");
        }
    }

    private string PromptFactory(Event @event, Notification notification)
    {
        if (@event.Type == EventType.TmNotification)
            return PromptForTmNotification(@event, notification);
        if (@event.Type == EventType.ReviewerNewNotification)
            return PromptForReviewerNewNotification(@event, notification);
        if (@event.Type == EventType.ReviewerReminderNotification)
            return PromptForReviewerReminderNotification(@event, notification);
        // Add more combinations as needed
        return string.Empty;
    }

    private string PromptForTmNotification(Event @event, Notification notification)
    {
        var doc = System.Text.Json.JsonDocument.Parse(@event.ContentJson!);
        var applicationUrl = doc.RootElement.GetProperty("ApplicationLink").GetString();
        var startDate = doc.RootElement.GetProperty("StartDate").GetString();
        var talentName = @event.ForName ?? "the talent";
        return $@"Write a polite email message to a Talent Mentor, informing them that it is time for the performance review of their talent named {talentName}.
            Ask them to initiate the review event at the following application URL: {applicationUrl}.
            The review should be started no later than this date: {startDate}.
            Be formal, clear, and appreciative in your tone. End the message with a thank you and a professional closing. Do not include a subject line.";
    }

    private string PromptForReviewerNewNotification(Event @event, Notification notification)
    {
        var doc = System.Text.Json.JsonDocument.Parse(@event.ContentJson!);
        var applicationUrl = doc.RootElement.GetProperty("ApplicationLink").GetString();
        var endDate = doc.RootElement.GetProperty("StartDate").GetString();
        var talentName = @event.ForName ?? "the talent";
        return $@"Write a polite email message to a colleague of {talentName}, asking them to provide feedback for the performance review.
            The feedback form is available at the following link: {applicationUrl}.
            The feedback must be submitted no later than: {endDate}.
            Be formal, clear, and appreciative in your tone. End the message with a thank you and a professional closing. Do not include a subject line.";
    }

    private string PromptForReviewerReminderNotification(Event @event, Notification notification)
    {
        var doc = System.Text.Json.JsonDocument.Parse(@event.ContentJson!);
        var applicationUrl = doc.RootElement.GetProperty("ApplicationLink").GetString();
        var endDate = doc.RootElement.GetProperty("StartDate").GetString();
        var talentName = @event.ForName ?? "the talent";
        int attempt = notification.Attempt;

        string recipient = $"Write an email message to a colleague of {talentName}, ";
        string formInfo = $@"The feedback form is available at the following link: {applicationUrl}.
            The feedback must be submitted no later than: {endDate}.
            ";

        if (attempt <= 2)
        {
            return $@"{recipient}reminding them to provide feedback for the performance review.
                {formInfo}Be formal, clear, and appreciative in your tone. End the message with a thank you and a professional closing. Do not include a subject line.";
        }
        if (attempt == 3 || attempt == 4)
        {
            return $@"{recipient}reminding them again to provide feedback for the performance review.
                {formInfo}Be more insistent and less polite, but still professional. Stress the importance of timely completion. Do not include a subject line.";
        }
        // attempt >= 5
        return $@"{recipient}warning them that they have not provided feedback for the performance review despite multiple reminders.
                {formInfo}If the feedback is not submitted immediately, this situation will be escalated to upper management. Be strict and formal, and emphasize the consequences of further delay. Do not include a subject line.";
    }
}
