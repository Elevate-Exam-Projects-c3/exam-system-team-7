using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.ForgotPassword.Dtos;
using exam_system.Features.Identity.ForgotPassword.Queries;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.ForgotPassword.Handlers;

// Newest password-reset code lookup; maps the row to the DTO.
public class GetLatestPasswordResetOtpByEmailQueryHandler
    : IRequestHandler<GetLatestPasswordResetOtpByEmailQuery, PasswordResetOtpDto?>
{
    private readonly IGenericRepository<PasswordResetOtp> _otps;

    public GetLatestPasswordResetOtpByEmailQueryHandler(IGenericRepository<PasswordResetOtp> otps)
    {
        _otps = otps;
    }

    public async Task<PasswordResetOtpDto?> Handle(GetLatestPasswordResetOtpByEmailQuery request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var query = _otps.Get(o => o.Email == normalizedEmail);

        if (request.UnusedOnly)
        {
            query = query.Where(o => !o.IsUsed);
        }

        var otp = await query
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return otp is null
            ? null
            : new PasswordResetOtpDto(otp.Id, otp.UserId, otp.OtpHash, otp.AttemptCount, otp.ExpiresAt, otp.CreatedAt);
    }
}
