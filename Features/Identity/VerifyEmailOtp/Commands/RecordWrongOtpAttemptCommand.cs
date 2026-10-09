using MediatR;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.VerifyEmailOtp.Commands;

// Increments one OtpCodes row's AttemptCount after a wrong submission.
public record RecordWrongOtpAttemptCommand(Guid OtpId)
    : IRequest<RequestResponse<int>>;
