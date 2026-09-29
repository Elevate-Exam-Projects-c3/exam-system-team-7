using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.ForgotPassword.Commands;

// Single-object mutation: increment ONE row's AttemptCount.
public record RecordWrongPasswordResetOtpAttemptCommand(Guid OtpId)
    : IRequest<RequestResponse<int>>;
