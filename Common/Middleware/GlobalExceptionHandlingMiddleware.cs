using FluentValidation;
using exam_system.Features.Shared;

namespace exam_system.Common.Middleware;

// Maps ValidationException to 400 with field errors, anything else to 500.
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
