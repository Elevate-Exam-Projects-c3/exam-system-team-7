using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Register.Orchestrators;

// OTP verification request; handled by VerifyOtpOrchestrator.
public record VerifyOtpCommand(string Email, string Code)
    : IRequest<RequestResponse<Guid>>;
