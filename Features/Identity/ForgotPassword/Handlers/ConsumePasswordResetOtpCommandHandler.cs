using MediatR;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.ForgotPassword.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.ForgotPassword.Handlers;

// ONE row: IsUsed=true (the OTP part dies) and the reset-token columns are
// stamped so the same row carries Step 3's credential.
public class ConsumePasswordResetOtpCommandHandler
    : IRequestHandler<ConsumePasswordResetOtpCommand, RequestResponse<Guid>>
{
    private readonly IGenericRepository<PasswordResetOtp> _otps;
    private readonly IUnitOfWork _unitOfWork;

    public ConsumePasswordResetOtpCommandHandler(
        IGenericRepository<PasswordResetOtp> otps,
        IUnitOfWork unitOfWork)
    {
        _otps = otps;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<Guid>> Handle(ConsumePasswordResetOtpCommand request, CancellationToken cancellationToken)
    {
        var otp = await _otps.GetByIdAsync(request.OtpId)
            ?? throw new InvalidOperationException($"PasswordResetOtps row {request.OtpId} was not found.");

        otp.IsUsed = true;
        otp.ResetToken = request.ResetToken;
        otp.ResetTokenExpiresAt = request.ResetTokenExpiresAt;

        _otps.Update(otp);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<Guid>.Ok(otp.Id);
    }
}
