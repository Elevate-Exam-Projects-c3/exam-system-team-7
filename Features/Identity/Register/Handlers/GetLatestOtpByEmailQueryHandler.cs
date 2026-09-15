using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Register.Dtos;
using exam_system.Features.Identity.Register.Queries;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Register.Handlers;

// Newest code lookup; maps the row to the DTO.
public class GetLatestOtpByEmailQueryHandler
    : IRequestHandler<GetLatestOtpByEmailQuery, OtpCodeDto?>
{
    private readonly IGenericRepository<EmailVerificationOtp> _otps;

    public GetLatestOtpByEmailQueryHandler(IGenericRepository<EmailVerificationOtp> otps)
    {
        _otps = otps;
    }

    public async Task<OtpCodeDto?> Handle(GetLatestOtpByEmailQuery request, CancellationToken cancellationToken)
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
            : new OtpCodeDto(otp.Id, otp.UserId, otp.OtpHash, otp.AttemptCount, otp.ExpiresAt, otp.CreatedAt);
    }
}
