using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace exam_system.Common.Middleware;

// EXAM-109 — the named "auth" rate-limiting policy, referenced by
// Program.cs (AddRateLimiter) and applied to the four auth endpoints with
// .RequireRateLimiting("auth"). It counts per client IP with a fixed window
// of 10 requests per minute; the OnRejected handler in Program.cs turns
// rejections into 429 + Retry-After (60s) + our RequestResponse shape.
public class AuthRateLimitPolicy : IRateLimiterPolicy<string>
{
    public const string PolicyName = "auth";

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; }

    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        // Partition key = client IP. All requests from one IP share the
        // 10/minute window across ALL auth endpoints — one attacker cannot
        // multiply the limit by mixing endpoints.
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,                    // story rule: 10 requests
                Window = TimeSpan.FromMinutes(1),    // story rule: per minute
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0                       // no queueing — reject immediately
            });
    }
}
