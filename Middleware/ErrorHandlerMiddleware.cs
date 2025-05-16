using System.Net;
using System.Text.Json;

namespace Zion.Reminder.Middleware;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            
            // Default to internal server error
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            
            // Customize status code based on exception type
            switch (error)
            {
                case ArgumentException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    _logger.LogWarning("Validation error: {ErrorMessage}", error.Message);
                    break;
                    
                case KeyNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    _logger.LogWarning("Resource not found: {ErrorMessage}", error.Message);
                    break;
                    
                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    _logger.LogWarning("Unauthorized access: {ErrorMessage}", error.Message);
                    break;
                    
                default:
                    // Log the full exception for internal server errors
                    _logger.LogError(error, "Error processing request: {ErrorMessage}", error.Message);
                    break;
            }

            // Create consistent error response format
            var errorResponse = new
            {
                success = false,
                message = error.Message,
                statusCode = response.StatusCode
            };
            
            var result = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(result);
        }
    }
}
