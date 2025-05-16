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
    private readonly OpenAISettings _openAISettings;

    public MessageGenerator(IOpenAIService openAiService, OpenAISettings openAISettings)
    {
        _openAiService = openAiService;
        _openAISettings = openAISettings;
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
            Model = _openAISettings.Model
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
        switch (@event.Type)
        {
            case EventType.TmNotification:
                return PromptForTmNotification(@event);
            case EventType.ReviewerNewNotification:
                return PromptForReviewerNewNotification(@event);
            case EventType.ReviewerReminderNotification:
                return PromptForReviewerReminderNotification(@event, notification);
            // Add more combinations as needed
            default:
                return string.Empty;
        }
    }

    private string BuildPrompt(string prompt, string contentJson, Event @event, Dictionary<string, string>? additionalProperties = null)
    {
        var safeContentJson = contentJson ?? string.Empty;
        var result = $"{_openAISettings.BasePrompt}\n{prompt}\nJSON data: {safeContentJson}";
        if (!string.IsNullOrEmpty(@event.ForName))
            result += $"\nForName: {@event.ForName}";
        if (!string.IsNullOrEmpty(@event.ToName))
            result += $"\nToName: {@event.ToName}";
        if (!string.IsNullOrEmpty(@event.FromName))
            result += $"\nFromName: {@event.FromName}";
        if (additionalProperties != null)
        {
            foreach (var kv in additionalProperties)
            {
                result += $"\n{kv.Key}: {kv.Value}";
            }
        }
        return result;
    }

    private string PromptForTmNotification(Event @event)
    {
        return BuildPrompt(_openAISettings.TmNotificationPrompt, @event.ContentJson!, @event);
    }

    private string PromptForReviewerNewNotification(Event @event)
    {
        return BuildPrompt(_openAISettings.ReviewerNewNotificationPrompt, @event.ContentJson!, @event);
    }

    private string PromptForReviewerReminderNotification(Event @event, Notification notification)
    {
        return BuildPrompt(_openAISettings.ReviewerReminderNotificationPrompt, @event.ContentJson!, @event, new Dictionary<string, string> { { "Attempts", notification.Attempt.ToString() } });
    }
}
