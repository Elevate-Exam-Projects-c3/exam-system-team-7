namespace exam_system.Features.Identity.Shared;

// Decorator over ANY IEmailSender (EXAM-113): retries transient provider
// failures with a short exponential backoff, then lets the last error
// bubble so the caller can answer the client with a generic failure.
// Wrapping instead of editing the concrete sender keeps the resilience
// working for every implementation (SMTP, SendGrid, Dev) — same reason
// ValidationBehavior wraps the MediatR pipeline.
public class ResilientEmailSender : IEmailSender
{
    private const int MaxAttempts = 3;

    // Growing pause between attempts: give a briefly-overloaded provider
    // room to recover instead of hammering it.
    private static readonly TimeSpan[] BackoffDelays =
    {
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(2)
    };

    private readonly IEmailSender _inner;
    private readonly ILogger<ResilientEmailSender> _logger;

    public ResilientEmailSender(IEmailSender inner, ILogger<ResilientEmailSender> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken)
        => SendWithRetryAsync(to, subject, body, htmlBody: null, cancellationToken);

    public Task SendEmailAsync(string to, string subject, string body, string htmlBody, CancellationToken cancellationToken)
        => SendWithRetryAsync(to, subject, body, htmlBody, cancellationToken);

    private async Task SendWithRetryAsync(string to, string subject, string body, string? htmlBody, CancellationToken cancellationToken)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                if (htmlBody is null)
                {
                    await _inner.SendEmailAsync(to, subject, body, cancellationToken);
                }
                else
                {
                    await _inner.SendEmailAsync(to, subject, body, htmlBody, cancellationToken);
                }

                return;
            }
            // Only the attempts that still have a next try are caught — the
            // final failure escapes to the caller. A cancelled request is
            // never retried.
            catch (Exception ex) when (attempt < MaxAttempts && ex is not OperationCanceledException)
            {
                var delay = BackoffDelays[attempt - 1];
                _logger.LogWarning(
                    ex,
                    "Email delivery to {To} failed (attempt {Attempt}/{MaxAttempts}); retrying in {DelaySeconds}s",
                    to, attempt, MaxAttempts, delay.TotalSeconds);

                await Task.Delay(delay, cancellationToken);
            }
        }
    }
}
