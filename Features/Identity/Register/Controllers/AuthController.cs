using MediatR;
using Microsoft.AspNetCore.Mvc;
using exam_system.Features.Identity.Register.Orchestrators;

namespace exam_system.Features.Identity.Register.Controllers;

// Thin controller (EXAM-103): it injects IMediator and forwards the request —
// zero business logic here. The MediatR pipeline (validation → orchestrator)
// does the work; the controller only maps the response to HTTP.
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST /api/auth/register — body: { fullName, email, password }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST /api/auth/verify-otp — body: { email, code } (EXAM-104)
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    // POST /api/auth/resend-otp — body: { email } (EXAM-2 internal subtask)
    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
