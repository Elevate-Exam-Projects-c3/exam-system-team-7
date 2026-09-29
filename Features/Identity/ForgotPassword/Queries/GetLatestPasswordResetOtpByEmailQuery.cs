using MediatR;
using exam_system.Features.Identity.ForgotPassword.Dtos;

namespace exam_system.Features.Identity.ForgotPassword.Queries;

// Newest password-reset code for an email; UnusedOnly targets the latest unused one.
public record GetLatestPasswordResetOtpByEmailQuery(string Email, bool UnusedOnly = false)
    : IRequest<PasswordResetOtpDto?>;
