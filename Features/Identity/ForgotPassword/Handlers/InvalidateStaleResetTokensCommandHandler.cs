using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.ForgotPassword.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.ForgotPassword.Handlers;

// Clears the reset-token columns on every row of the account that still
// carries a live token; the revoked count travels back for logging.
public class InvalidateStaleResetTokensCommandHandler
    : IRequestHandler<InvalidateStaleResetTokensCommand, RequestResponse<int>>
{
    private readonly IGenericRepository<PasswordResetOtp> _otps;
    private readonly IUnitOfWork _unitOfWork;

    public InvalidateStaleResetTokensCommandHandler(
        IGenericRepository<PasswordResetOtp> otps,
        IUnitOfWork unitOfWork)
    {
        _otps = otps;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<int>> Handle(InvalidateStaleResetTokensCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var live = await _otps
            .Get(o => o.Email == normalizedEmail && o.ResetToken != null)
            .ToListAsync(cancellationToken);

        foreach (var row in live)
        {
            row.ResetToken = null;
            row.ResetTokenExpiresAt = null;
            _otps.Update(row);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<int>.Ok(live.Count);
    }
}
