using MediatR;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Register.Handlers;

// Increments one OtpCodes row's AttemptCount and returns the new count.
public class RecordWrongOtpAttemptCommandHandler
    : IRequestHandler<RecordWrongOtpAttemptCommand, RequestResponse<int>>
{
    private readonly IGenericRepository<EmailVerificationOtp> _otps;
    private readonly IUnitOfWork _unitOfWork;

    public RecordWrongOtpAttemptCommandHandler(
        IGenericRepository<EmailVerificationOtp> otps,
        IUnitOfWork unitOfWork)
    {
        _otps = otps;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<int>> Handle(RecordWrongOtpAttemptCommand request, CancellationToken cancellationToken)
    {
        var otp = await _otps.GetByIdAsync(request.OtpId)
            ?? throw new InvalidOperationException($"OtpCodes row {request.OtpId} was not found.");

        otp.AttemptCount++;
        _otps.Update(otp);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // The new count travels back so the Orchestrator can choose between
        // the locked and the generic message without re-reading the row.
        return RequestResponse<int>.Ok(otp.AttemptCount);
    }
}
