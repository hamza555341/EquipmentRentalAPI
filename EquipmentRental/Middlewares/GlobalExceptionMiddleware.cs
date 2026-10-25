
using EquipmentRental.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentRental.Api.Middlewares;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            // Handle unmatched endpoints that returned an empty 404.
            if (context.Response.StatusCode == StatusCodes.Status404NotFound
                && !context.Response.HasStarted
                && context.Response.ContentLength is null)
            {
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Title = "Endpoint not found",
                    Detail = $"No endpoint was found for '{context.Request.Path}'.",
                    Status = StatusCodes.Status404NotFound,
                    Instance = context.Request.Path
                };

                await context.Response.WriteAsJsonAsync(problem);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unhandled exception occurred while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.ContentType = "application/problem+json";

            var statusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            var problem = new ProblemDetails
            {
                Title = statusCode == StatusCodes.Status404NotFound
                    ? "Resource not found"
                    : "An unexpected error occurred",
                Status = statusCode,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
