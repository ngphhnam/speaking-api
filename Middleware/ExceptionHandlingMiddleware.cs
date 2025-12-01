using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace SpeakingPractice.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex, logger);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var traceId = context.TraceIdentifier;
        
        string errorMessage = exception.Message;
        string? innerException = null;
        string? details = null;

        // Handle DbUpdateException specifically to get inner exception details
        if (exception is DbUpdateException dbEx)
        {
            errorMessage = "An error occurred while saving the entity changes.";
            innerException = dbEx.InnerException?.Message;
            
            // Try to get more details from the inner exception
            if (dbEx.InnerException != null)
            {
                details = dbEx.InnerException.ToString();
                logger.LogError(dbEx.InnerException, "Database update exception inner details");
            }
        }

        var response = new
        {
            error = errorMessage,
            innerException,
            details = details?.Split('\n').FirstOrDefault(), // First line of details for brevity
            traceId
        };

        var payload = JsonSerializer.Serialize(response);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(payload);
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}

