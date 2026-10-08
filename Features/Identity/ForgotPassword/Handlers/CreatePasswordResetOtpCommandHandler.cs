using MediatR;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.ForgotPassword.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.ForgotPassword.Handlers;

// Inserts one PasswordResetOtps row; the code arrives already hashed.
public class CreatePasswordResetOtpCommandHandler
    : IRequestHandler<CreatePasswordResetOtpCommand, RequestResponse<Guid>>
{
    private readonly IGenericRepository<PasswordResetOtp> _otps;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePasswordResetOtpCommandHandler(
        IGenericRepository<PasswordResetOtp> otps,
        IUnitOfWork unitOfWork)
    {
        _otps = otps;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<Guid>> Handle(CreatePasswordResetOtpCommand request, CancellationToken cancellationToken)
    {
        var otp = new PasswordResetOtp
        {
            UserId = request.UserId,
            Email = request.Email,
            OtpHash = request.OtpHash,
            ExpiresAt = request.ExpiresAt,
            AttemptCount = 0,
            IsUsed = false
        };

        await _otps.AddAsync(otp);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<Guid>.Ok(otp.Id);
    }
}
