using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using exam_system.Features.Identity.Login.Orchestrators;
using exam_system.Features.Identity.Shared;

namespace exam_system.Features.Identity.Login.Controllers;

// Login endpoint; issues the access token in the body and the refresh
// token as an httpOnly cookie.
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

    // POST /api/auth/login — body: { email, password }
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);

        if (response.Success && response.Data is not null)
        {
            // httpOnly so client-side JavaScript cannot read the refresh token.
            Response.Cookies.Append(
                _jwtOptions.CookieName,
                response.Data.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // dev only; must be true behind HTTPS
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)
                });
        }

        return StatusCode(response.StatusCode, response);
    }
}
