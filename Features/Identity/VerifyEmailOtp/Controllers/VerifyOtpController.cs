using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using exam_system.Features.Identity.VerifyEmailOtp.Orchestrators;

namespace exam_system.Features.Identity.VerifyEmailOtp.Controllers;

// Verify endpoint; activates the account once the OTP matches.
[ApiController]
[Route("api/auth")]
public class VerifyOtpController : ControllerBase
{
    private readonly IMediator _mediator;

    public VerifyOtpController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST /api/auth/verify-otp — body: { email, code }
    [HttpPost("verify-otp")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
