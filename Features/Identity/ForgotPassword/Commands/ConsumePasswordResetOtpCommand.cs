using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.ForgotPassword.Commands;

// Single-object mutation: ONE PasswordResetOtps row flips to used AND
// starts carrying the reset token (sent only after the code passed
// every gate).
public record ConsumePasswordResetOtpCommand(Guid OtpId, string ResetToken, DateTime ResetTokenExpiresAt)
    : IRequest<RequestResponse<Guid>>;
