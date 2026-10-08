using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.ForgotPassword.Commands;

// Creates one PasswordResetOtps row; the code arrives already hashed.
public record CreatePasswordResetOtpCommand(Guid UserId, string Email, string OtpHash, DateTime ExpiresAt)
    : IRequest<RequestResponse<Guid>>;
