using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Register.Commands;

// Single-object mutation: mark ONE OtpCodes row as used. The Orchestrator
// sends it only AFTER the code passed every gate (hash, expiry, lock).
public record ConsumeEmailVerificationOtpCommand(Guid OtpId)
    : IRequest<RequestResponse<Guid>>;
