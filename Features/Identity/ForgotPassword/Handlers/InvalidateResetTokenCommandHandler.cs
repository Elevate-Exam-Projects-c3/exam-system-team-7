using MediatR;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.ForgotPassword.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.ForgotPassword.Handlers;

// Clears ONE row's reset-token columns; a replayed token then finds no row.
public class InvalidateResetTokenCommandHandler
    : IRequestHandler<InvalidateResetTokenCommand, RequestResponse>
{
    private readonly IGenericRepository<PasswordResetOtp> _otps;
    private readonly IUnitOfWork _unitOfWork;

    public InvalidateResetTokenCommandHandler(
        IGenericRepository<PasswordResetOtp> otps,
        IUnitOfWork unitOfWork)
    {
        _otps = otps;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse> Handle(InvalidateResetTokenCommand request, CancellationToken cancellationToken)
    {
        var otp = await _otps.GetByIdAsync(request.OtpId)
            ?? throw new InvalidOperationException($"PasswordResetOtps row {request.OtpId} was not found.");

        otp.ResetToken = null;
        otp.ResetTokenExpiresAt = null;

        _otps.Update(otp);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse.Ok("Reset token invalidated.");
    }
}
