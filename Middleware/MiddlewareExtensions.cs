namespace Zion.Reminder.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandler(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlerMiddleware>();
    }
}
