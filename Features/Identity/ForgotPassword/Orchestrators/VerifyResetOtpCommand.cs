using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.ForgotPassword.Orchestrators;

// Step 2 request; handled by VerifyResetOtpOrchestrator.
public record VerifyResetOtpCommand(string Email, string Code)
    : IRequest<RequestResponse<string>>;
