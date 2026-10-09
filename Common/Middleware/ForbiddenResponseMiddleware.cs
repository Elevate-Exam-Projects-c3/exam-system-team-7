using exam_system.Features.Shared;

namespace exam_system.Common.Middleware;

// Rewrites the empty 403 body produced by the authorization middleware
// into the standard RequestResponse shape, so every client gets one
// consistent error contract (401/403/429 all look alike).
public class ForbiddenResponseMiddleware
{
    private readonly RequestDelegate _next;

    public ForbiddenResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status403Forbidden
            && !context.Response.HasStarted)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(
                RequestResponse<object>.Fail(
                    "You do not have permission to access this resource.",
                    403));
        }
    }
}
