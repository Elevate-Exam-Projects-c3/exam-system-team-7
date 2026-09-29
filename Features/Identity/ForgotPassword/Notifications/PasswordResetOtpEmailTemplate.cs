using exam_system.Features.Identity.Shared;

namespace exam_system.Features.Identity.ForgotPassword.Notifications;

// The password-reset email: only the variable pieces live here — the
// branded shell comes from EmailTemplateBase.
public sealed class PasswordResetOtpEmailTemplate : EmailTemplateBase
{
    public PasswordResetOtpEmailTemplate(string code, int lifetimeMinutes)
        : base(
            subject: "Reset your password",
            title: "Reset your password",
            message: $"Use this code to set a new password. It expires in {lifetimeMinutes} minutes.",
            highlight: code,
            footnote: "If you did not request a password reset, you can safely ignore this email.")
    {
    }
}
