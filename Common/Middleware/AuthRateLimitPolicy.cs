using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace exam_system.Common.Middleware;

// Rate-limiting policy for the auth endpoints: 10 requests/minute per
// client IP. Rejections return 429 + Retry-After (configured in Program.cs).
public class AuthRateLimitPolicy : IRateLimiterPolicy<string>
{
    public const string PolicyName = "auth";

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; }

    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        // All auth requests from one IP share the same window.
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0                       // reject immediately, no queueing
            });
    }
}
