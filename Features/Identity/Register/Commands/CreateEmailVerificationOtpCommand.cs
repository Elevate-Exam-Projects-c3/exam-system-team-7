using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Register.Commands;

// One OtpCodes row = one object mutation. The Orchestrator decides WHEN to
// create it and passes the ALREADY-HASHED code — the plain OTP never reaches
// this Command (it is emailed, not stored).
public record CreateEmailVerificationOtpCommand(Guid UserId, string Email, string OtpHash, DateTime ExpiresAt)
    : IRequest<RequestResponse<Guid>>;
