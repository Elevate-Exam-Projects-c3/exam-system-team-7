using MediatR;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Register.Handlers;

// Single responsibility: flip ONE OtpCodes row to IsUsed=true.
public class ConsumeEmailVerificationOtpCommandHandler
    : IRequestHandler<ConsumeEmailVerificationOtpCommand, RequestResponse<Guid>>
{
    private readonly IGenericRepository<EmailVerificationOtp> _otps;
    private readonly IUnitOfWork _unitOfWork;

    public ConsumeEmailVerificationOtpCommandHandler(
        IGenericRepository<EmailVerificationOtp> otps,
        IUnitOfWork unitOfWork)
    {
        _otps = otps;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<Guid>> Handle(ConsumeEmailVerificationOtpCommand request, CancellationToken cancellationToken)
    {
        var otp = await _otps.GetByIdAsync(request.OtpId)
            ?? throw new InvalidOperationException($"OtpCodes row {request.OtpId} was not found.");

        otp.IsUsed = true;
        _otps.Update(otp);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<Guid>.Ok(otp.Id);
    }
}
