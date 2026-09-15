using MediatR;
using Microsoft.Extensions.Options;
using exam_system.Features.Identity.Login.Commands;
using exam_system.Features.Identity.RefreshToken.Commands;
using exam_system.Features.Identity.RefreshToken.Dtos;
using exam_system.Features.Identity.RefreshToken.Queries;
using exam_system.Features.Identity.Shared;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.RefreshToken.Orchestrators;

// Refresh flow: gates + rotation, both writes in one transaction.
public class RefreshTokenOrchestrator
    : IRequestHandler<RefreshTokenCommand, RequestResponse<RefreshResponse>>
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtOptions _jwtOptions;

    public RefreshTokenOrchestrator(
        IMediator mediator,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        IOptions<JwtOptions> jwtOptions)
    {
        _mediator = mediator;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<RequestResponse<RefreshResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Invalid("Invalid refresh token.");
        }

        var row = await _mediator.Send(new GetRefreshTokenByTokenQuery(request.RefreshToken), cancellationToken);

        if (row is null)
        {
            return Invalid("Invalid refresh token.");
        }

        // A used token coming back = replay; reject it.
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

        var accessToken = _tokenService.GenerateAccessToken(row.UserId, row.UserRole);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Rotation is two writes, so they go in one transaction.
        await _unitOfWork.BeginTransactionAsync();
        var committed = false;
        try
        {
            await _mediator.Send(
                new ConsumeRefreshTokenCommand(row.Id, newRefreshToken),
                cancellationToken);

            await _mediator.Send(
                new CreateRefreshTokenCommand(
                    row.UserId,
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

    // Every refresh rejection is an authentication failure.
    private static RequestResponse<RefreshResponse> Invalid(string message)
    {
        return RequestResponse<RefreshResponse>.Fail(message, 401);
    }
}
