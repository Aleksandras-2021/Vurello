namespace PSK.Server.Middlewares;

using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (ArgumentNullException ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status400BadRequest, "400 Bad Request - Missing argument", ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status403Forbidden, "403 Forbidden - Unauthorized access", ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status404NotFound, "404 Not Found - Resource not found", ex.Message);
        }
        catch (DbException ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status500InternalServerError, "500 Internal Server Error - Database error", "A database error occurred.");
        }
        catch (InvalidOperationException ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status409Conflict, "409 Conflict - Invalid operation", ex.Message);
        }
        catch (NotSupportedException ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status405MethodNotAllowed, "405 Method Not Allowed - Unsupported operation", ex.Message);
        }
        catch (TimeoutException ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status504GatewayTimeout, "504 Gateway Timeout - Timeout occurred", "The request timed out. Please try again later.");
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, StatusCodes.Status500InternalServerError, "500 Internal Server Error", "An unexpected error occurred.");
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex, int statusCode, string logPrefix, string responseMessage)
    {
        var controllerAction = GetControllerAndAction(context);
        string logMessage = $"{logPrefix} in {controllerAction}. Message: {ex.Message}";

        _logger.LogWarning(logMessage);

        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = responseMessage,
            Detail = ex.Message,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    }


    private string GetControllerAndAction(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>() is { } descriptor)
        {
            return $"{descriptor.ControllerName}.{descriptor.ActionName}";
        }

        return "Unknown Controller/Action";
    }
}