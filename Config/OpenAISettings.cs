// Settings for OpenAI integration
using Microsoft.Extensions.Configuration;

public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-3.5-turbo";

    public string BasePrompt { get; set; } = string.Empty;
    public string TmNotificationPrompt { get; set; } = string.Empty;
    public string ReviewerNewNotificationPrompt { get; set; } = string.Empty;
    public string ReviewerReminderNotificationPrompt { get; set; } = string.Empty;

    public static OpenAISettings FromConfiguration(IConfiguration configuration)
    {
        var section = configuration.GetSection("OpenAI");        var settings = section.Get<OpenAISettings>() ?? new OpenAISettings();

        // Get the prompts section
        var promptsSection = section.GetSection("MessageGeneratorPrompts");

        // Find prompts inside MessageGeneratorPrompts section
        settings.BasePrompt = ConcatPrompt(promptsSection, "BasePrompt", settings.BasePrompt);
        settings.TmNotificationPrompt = ConcatPrompt(promptsSection, "TmNotificationPrompt", settings.TmNotificationPrompt);
        settings.ReviewerNewNotificationPrompt = ConcatPrompt(promptsSection, "ReviewerNewNotificationPrompt", settings.ReviewerNewNotificationPrompt);
        settings.ReviewerReminderNotificationPrompt = ConcatPrompt(promptsSection, "ReviewerReminderNotificationPrompt", settings.ReviewerReminderNotificationPrompt);

        return settings;
    }

    private static string ConcatPrompt(IConfigurationSection section, string key, string fallback)
    {
        var arr = section.GetSection(key).Get<string[]>();
        if (arr != null && arr.Length > 0)
            return string.Join("\n", arr);
        return fallback ?? string.Empty;
    }
}
