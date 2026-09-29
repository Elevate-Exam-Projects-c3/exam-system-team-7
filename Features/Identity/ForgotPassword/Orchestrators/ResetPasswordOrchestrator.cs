using MediatR;
using exam_system.Features.Identity.ForgotPassword.Commands;
using exam_system.Features.Identity.ForgotPassword.Queries;
using exam_system.Features.Identity.Shared;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;

namespace exam_system.Features.Identity.ForgotPassword.Orchestrators;

// Step 3 of the reset flow: the reset token buys ONE password change.
// Password update + token invalidation + revoking EVERY session land in
// one transaction — the "log out everywhere" guarantee of the story.
public class ResetPasswordOrchestrator : IRequestHandler<ResetPasswordCommand, RequestResponse>
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordOrchestrator(
        IMediator mediator,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<RequestResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // The reset token identifies the row (no email in Step 3's body).
        var otp = await _mediator.Send(
            new GetPasswordResetOtpByTokenQuery(request.ResetToken),
            cancellationToken);

        // Unknown token, cleared token, or an expired window: ONE generic
        // 401 — the token is the flow's credential, so rejections keep the
        // refresh-gates rule (authentication semantics, never 400).
        if (otp is null)
        {
            return RequestResponse.Fail("Invalid or expired reset token.", 401);
        }

        if (otp.ResetTokenExpiresAt is null || otp.ResetTokenExpiresAt <= DateTime.UtcNow)
        {
            return RequestResponse.Fail("Invalid or expired reset token.", 401);
        }

        // The plain password lives only here; only the hash travels on.
        var passwordHash = _passwordHasher.Hash(request.NewPassword);

        // THE ticket's transaction: a password change whose sessions
        // survive is a broken reset — all three writes move together.
        await _unitOfWork.BeginTransactionAsync();
        var committed = false;
        try
        {
            await _mediator.Send(new UpdateUserPasswordCommand(otp.UserId, passwordHash), cancellationToken);

            // Invalidation is literal: the token column is cleared, so a
            // replay finds no row at all.
            await _mediator.Send(new InvalidateResetTokenCommand(otp.Id), cancellationToken);

            // A stolen refresh token cannot survive its owner's reset.
            await _mediator.Send(new RevokeAllRefreshTokensCommand(otp.UserId), cancellationToken);

            await _unitOfWork.CommitTransactionAsync();
            committed = true;

            return RequestResponse.Ok(
                "Password reset successful. Please log in with your new password.");
        }
        finally
        {
            if (!committed)
            {
                await _unitOfWork.RollbackTransactionAsync();
            }
        }
    }
}
