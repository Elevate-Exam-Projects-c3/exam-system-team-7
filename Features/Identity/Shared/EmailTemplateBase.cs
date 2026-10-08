namespace exam_system.Features.Identity.Shared;

// Template Method pattern: ONE branded HTML shell for every email we send.
// Concrete templates supply only the variable pieces (title, message,
// highlighted value, footnote) — a new email type subclasses this instead
// of copy-pasting the layout. Inline styles only: most email clients strip
// <style> blocks.
public abstract class EmailTemplateBase
{
    public string Subject { get; }
    public string PlainBody { get; }
    public string HtmlBody { get; }

    protected EmailTemplateBase(
        string subject,
        string title,
        string message,
        string? highlight = null,
        string? footnote = null)
    {
        Subject = subject;

        // Plain-text fallback mirrors the HTML content.
        PlainBody = highlight is null
            ? message
            : $"{message}\n\nCode: {highlight}";

        HtmlBody = BuildHtml(title, message, highlight, footnote);
    }

    private static string BuildHtml(string title, string message, string? highlight, string? footnote)
    {
        var highlightHtml = highlight is null
            ? string.Empty
            : $"""
              <div style="font-size:32px;letter-spacing:8px;font-weight:bold;color:#111827;background:#f3f4f6;border-radius:8px;padding:16px 0;margin:0 0 16px;">{highlight}</div>
              """;

        var footnoteHtml = footnote is null
            ? string.Empty
            : $"""
              <p style="color:#9ca3af;font-size:12px;margin:24px 0 0;">{footnote}</p>
              """;

        return $"""
            <div style="font-family:Arial,Helvetica,sans-serif;max-width:480px;margin:0 auto;padding:24px;background:#f6f7fb;">
              <div style="background:#ffffff;border-radius:8px;padding:32px;text-align:center;">
                <h2 style="color:#1f2937;margin:0 0 8px;">{title}</h2>
                <p style="color:#4b5563;margin:0 0 24px;">{message}</p>
                {highlightHtml}
                {footnoteHtml}
              </div>
            </div>
            """;
    }
}
