using NairaLedger.Application.Exceptions;
using FluentValidation;

namespace NairaLedger.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {ExceptionType} – {Message}", ex.GetType().Name, ex.Message);

            (HttpStatusCode statusCode, string message) = ex switch
            {
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, ex.Message),
                UserAlreadyExistsException => (HttpStatusCode.Conflict, ex.Message),
                ValidationException => (HttpStatusCode.BadRequest, ex.Message),
                InvalidOperationException when ex.Message.Contains("Insufficient") => (HttpStatusCode.BadRequest, ex.Message),
                InvalidOperationException when ex.Message.Contains("not found") => (HttpStatusCode.NotFound, ex.Message),
                InvalidOperationException when ex.Message.Contains("expired") => (HttpStatusCode.BadRequest, ex.Message),
                InvalidOperationException when ex.Message.Contains("cannot be reversed") => (HttpStatusCode.BadRequest, ex.Message),
                InvalidOperationException => (HttpStatusCode.BadRequest, ex.Message), // fallback for business rule violations
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
            };

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new { error = message });
            await context.Response.WriteAsync(result);
        }
    }
}