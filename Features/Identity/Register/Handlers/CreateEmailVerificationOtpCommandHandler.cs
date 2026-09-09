using MediatR;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Register.Handlers;

// Single responsibility: insert one OtpCodes row. No hashing here — the
// Orchestrator already hashed the code; this Command only persists state.
public class CreateEmailVerificationOtpCommandHandler
    : IRequestHandler<CreateEmailVerificationOtpCommand, RequestResponse<Guid>>
{
    private readonly IGenericRepository<EmailVerificationOtp> _otps;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmailVerificationOtpCommandHandler(
        IGenericRepository<EmailVerificationOtp> otps,
        IUnitOfWork unitOfWork)
    {
        _otps = otps;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<Guid>> Handle(CreateEmailVerificationOtpCommand request, CancellationToken cancellationToken)
    {
        var otp = new EmailVerificationOtp
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
