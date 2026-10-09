using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.ForgotPassword.Orchestrators;

// Step 3 request; handled by ResetPasswordOrchestrator.
public record ResetPasswordCommand(string ResetToken, string NewPassword, string ConfirmPassword)
    : IRequest<RequestResponse>;
