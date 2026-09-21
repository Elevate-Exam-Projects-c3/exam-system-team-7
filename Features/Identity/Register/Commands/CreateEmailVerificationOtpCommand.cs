using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Register.Commands;

// Creates one OtpCodes row; the code arrives already hashed.
public record CreateEmailVerificationOtpCommand(Guid UserId, string Email, string OtpHash, DateTime ExpiresAt)
    : IRequest<RequestResponse<Guid>>;
