using MediatR;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Login.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.Login.Handlers;

// Single responsibility: insert ONE RefreshTokens row (7-day TTL comes from
// the Orchestrator). No decisions here — pure state change.
public class CreateRefreshTokenCommandHandler
    : IRequestHandler<CreateRefreshTokenCommand, RequestResponse<Guid>>
{
    private readonly IGenericRepository<RefreshToken> _refreshTokens;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRefreshTokenCommandHandler(
        IGenericRepository<RefreshToken> refreshTokens,
        IUnitOfWork unitOfWork)
    {
        _refreshTokens = refreshTokens;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<Guid>> Handle(CreateRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = new RefreshToken
        {
            UserId = request.UserId,
            Token = request.Token,
            ExpiresAt = request.ExpiresAt,
            IsUsed = false,
            IsRevoked = false
        };

        await _refreshTokens.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<Guid>.Ok(refreshToken.Id);
    }
}
