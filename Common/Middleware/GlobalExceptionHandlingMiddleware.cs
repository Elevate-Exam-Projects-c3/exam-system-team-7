using FluentValidation;
using exam_system.Features.Shared;

namespace exam_system.Common.Middleware;

// Global exception handler: every controller (and minimal API) gets the same
// mapping — ValidationException -> 400 with field-mapped errors, anything
// else -> 500 without leaking details. This replaces the inline catch that
// used to live in the temporary test endpoint.
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(
                RequestResponse<object>.Fail("One or more validation errors occurred.", 400, errors));
        }
        catch (Exception)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(
                RequestResponse<object>.Fail("An unexpected error occurred.", 500));
        }
    }
}
