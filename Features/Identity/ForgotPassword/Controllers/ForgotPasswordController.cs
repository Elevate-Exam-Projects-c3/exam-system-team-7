using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using exam_system.Features.Identity.ForgotPassword.Orchestrators;

namespace exam_system.Features.Identity.ForgotPassword.Controllers;

// The three reset-flow endpoints: all public (the user forgot the
// password!) and rate-limited with the shared auth policy — forgot-password
// is the most email-bombing-prone endpoint we own.
[ApiController]
[Route("api/auth")]
public class ForgotPasswordController : ControllerBase
{
    private readonly IMediator _mediator;

    public ForgotPasswordController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST /api/auth/forgot-password — body: { email }
    [HttpPost("forgot-password")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST /api/auth/verify-reset-otp — body: { email, code }
    [HttpPost("verify-reset-otp")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> VerifyResetOtp([FromBody] VerifyResetOtpCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST /api/auth/reset-password — body: { resetToken, newPassword, confirmPassword }
    [HttpPost("reset-password")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
