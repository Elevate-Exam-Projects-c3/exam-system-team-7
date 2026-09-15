using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using exam_system.Features.Identity.Login.Commands;

// The RefreshToken slice's namespace shadows the entity name inside
// Features.Identity, so the entity gets an alias here.
using RefreshTokenEntity = exam_system.Domain.Entities.Identity.RefreshToken;
using exam_system.Features.Identity.RefreshToken.Commands;
using exam_system.Features.Identity.Shared;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.RefreshToken.Orchestrators;

// Refresh flow: validate the stored token (unused, unrevoked, unexpired),
// then rotate it — consume the old row and insert the new one in ONE
// transaction. Reuse of an already-used token is treated as a compromise
// signal and rejected.
public class RefreshTokenOrchestrator
    : IRequestHandler<RefreshTokenCommand, RequestResponse<RefreshResponse>>
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<RefreshTokenEntity> _refreshTokens;
    private readonly JwtOptions _jwtOptions;

    public RefreshTokenOrchestrator(
        IMediator mediator,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        IGenericRepository<RefreshTokenEntity> refreshTokens,
        IOptions<JwtOptions> jwtOptions)
    {
        _mediator = mediator;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _refreshTokens = refreshTokens;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<RequestResponse<RefreshResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Invalid("Invalid refresh token.");
        }

        // Plain equality on the unique token index; the value itself is the secret.
        var row = await _refreshTokens
            .Get(r => r.Token == request.RefreshToken)
            .Include(r => r.User)
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null)
        {
            return Invalid("Invalid refresh token.");
        }

        // A used token must never come back: either a replay attack or a
        // stolen pair racing the real client. Rejecting it kills the replay.
        if (row.IsUsed)
        {
            return Invalid("Refresh token has already been used.");
        }

        if (row.IsRevoked)
        {
            return Invalid("Refresh token has been revoked.");
        }

        if (row.ExpiresAt <= DateTime.UtcNow)
        {
            return Invalid("Refresh token has expired. Please log in again.");
        }

        var user = row.User;
        if (user is null)
        {
            return Invalid("Invalid refresh token.");
        }

        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Role.ToString());
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Rotation is two writes: consume the old row and insert the new one.
        await _unitOfWork.BeginTransactionAsync();
        var committed = false;
        try
        {
            await _mediator.Send(
                new ConsumeRefreshTokenCommand(row.Id, newRefreshToken),
                cancellationToken);

            await _mediator.Send(
                new CreateRefreshTokenCommand(
                    user.Id,
                    newRefreshToken,
                    DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)),
                cancellationToken);

            await _unitOfWork.CommitTransactionAsync();
            committed = true;

            return RequestResponse<RefreshResponse>.Ok(
                new RefreshResponse(accessToken, newRefreshToken),
                "Token refreshed.");
        }
        finally
        {
            if (!committed)
            {
                await _unitOfWork.RollbackTransactionAsync();
            }
        }
    }

    // Every refresh rejection is an authentication failure → 401.
    private static RequestResponse<RefreshResponse> Invalid(string message)
    {
        return RequestResponse<RefreshResponse>.Fail(message, 401);
    }
}
