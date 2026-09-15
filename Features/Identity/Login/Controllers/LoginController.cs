using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using exam_system.Features.Identity.Login.Orchestrators;
using exam_system.Features.Identity.Shared;

namespace exam_system.Features.Identity.Login.Controllers;

// Login endpoint; access token in the body, refresh token in the cookie.
[ApiController]
[Route("api/auth")]
public class LoginController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly JwtOptions _jwtOptions;

    public LoginController(IMediator mediator, Microsoft.Extensions.Options.IOptions<JwtOptions> jwtOptions)
    {
        _mediator = mediator;
        _jwtOptions = jwtOptions.Value;
    }

    // POST /api/auth/login
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);

        if (response.Success && response.Data is not null)
        {
            Response.SetRefreshTokenCookie(_jwtOptions, response.Data.RefreshToken);
        }

        return StatusCode(response.StatusCode, response);
    }
}
