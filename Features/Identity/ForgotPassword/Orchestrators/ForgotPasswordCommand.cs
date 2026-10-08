using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.ForgotPassword.Orchestrators;

// Step 1 request; handled by ForgotPasswordOrchestrator.
public record ForgotPasswordCommand(string Email)
    : IRequest<RequestResponse>;
