using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Register.Orchestrators;

// EXAM-104: the flow-entry request for email verification. It is handled by
// the ORCHESTRATOR (VerifyOtpOrchestrator) because the flow touches TWO
// aggregates: the OtpCodes row (IsUsed) and the ApplicationUser (activated).
// Same placement rule as RegisterUserCommand (convention #10).
public record VerifyOtpCommand(string Email, string Code)
    : IRequest<RequestResponse<Guid>>;
