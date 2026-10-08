using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Features.Identity.ForgotPassword.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using RefreshTokenEntity = exam_system.Domain.Entities.Identity.RefreshToken;

namespace exam_system.Features.Identity.ForgotPassword.Handlers;

// Revokes every live session of the user; zero rows = a no-op. The alias
// exists because the RefreshToken SLICE's namespace shadows the entity
// name inside Features.Identity (same lesson as the EXAM-5 restructure).
public class RevokeAllRefreshTokensCommandHandler
    : IRequestHandler<RevokeAllRefreshTokensCommand, RequestResponse<int>>
{
    private readonly IGenericRepository<RefreshTokenEntity> _tokens;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeAllRefreshTokensCommandHandler(
        IGenericRepository<RefreshTokenEntity> tokens,
        IUnitOfWork unitOfWork)
    {
        _tokens = tokens;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<int>> Handle(RevokeAllRefreshTokensCommand request, CancellationToken cancellationToken)
    {
        var live = await _tokens
            .Get(t => t.UserId == request.UserId && !t.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in live)
        {
            token.IsRevoked = true;
            _tokens.Update(token);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<int>.Ok(live.Count);
    }
}
