using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Register.Orchestrators;

// Resend request; handled by ResendOtpOrchestrator.
public record ResendOtpCommand(string Email)
    : IRequest<RequestResponse>;
