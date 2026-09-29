using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.ForgotPassword.Dtos;
using exam_system.Features.Identity.ForgotPassword.Queries;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.ForgotPassword.Handlers;

// Finds the row carrying this reset token; read-only, mapped to a DTO.
// Plain equality on a 256-bit value (no hashing needed at that entropy —
// same lesson as the refresh-token lookup).
public class GetPasswordResetOtpByTokenQueryHandler
    : IRequestHandler<GetPasswordResetOtpByTokenQuery, PasswordResetTokenDto?>
{
    private readonly IGenericRepository<PasswordResetOtp> _otps;

    public GetPasswordResetOtpByTokenQueryHandler(IGenericRepository<PasswordResetOtp> otps)
    {
        _otps = otps;
    }

    public async Task<PasswordResetTokenDto?> Handle(GetPasswordResetOtpByTokenQuery request, CancellationToken cancellationToken)
    {
        var otp = await _otps
            .Get(o => o.ResetToken == request.ResetToken)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return otp is null
            ? null
            : new PasswordResetTokenDto(otp.Id, otp.UserId, otp.ResetTokenExpiresAt);
    }
}
