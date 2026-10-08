using MediatR;
using exam_system.Features.Identity.ForgotPassword.Dtos;

namespace exam_system.Features.Identity.ForgotPassword.Queries;

// Step 3 lookup: the row currently carrying this reset token.
public record GetPasswordResetOtpByTokenQuery(string ResetToken)
    : IRequest<PasswordResetTokenDto?>;
