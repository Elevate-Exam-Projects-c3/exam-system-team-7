using MediatR;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.ForgotPassword.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.ForgotPassword.Handlers;

// Increments ONE PasswordResetOtps row's AttemptCount and returns the new
// count so the Orchestrator picks locked vs generic without re-reading.
public class RecordWrongPasswordResetOtpAttemptCommandHandler
    : IRequestHandler<RecordWrongPasswordResetOtpAttemptCommand, RequestResponse<int>>
{
    private readonly IGenericRepository<PasswordResetOtp> _otps;
    private readonly IUnitOfWork _unitOfWork;

    public RecordWrongPasswordResetOtpAttemptCommandHandler(
        IGenericRepository<PasswordResetOtp> otps,
        IUnitOfWork unitOfWork)
    {
        _otps = otps;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<int>> Handle(RecordWrongPasswordResetOtpAttemptCommand request, CancellationToken cancellationToken)
    {
        var otp = await _otps.GetByIdAsync(request.OtpId)
            ?? throw new InvalidOperationException($"PasswordResetOtps row {request.OtpId} was not found.");

        otp.AttemptCount++;
        _otps.Update(otp);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<int>.Ok(otp.AttemptCount);
    }
}
