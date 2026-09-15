using MediatR;
using Microsoft.AspNetCore.Mvc;
using exam_system.Features.Identity.Logout.Commands;
using exam_system.Features.Identity.Shared;
using exam_system.Features.Shared;

namespace exam_system.Features.Identity.Logout.Controllers;

// Logout revokes the cookie's refresh token and clears the cookie;
// always succeeds so logout is idempotent.
[ApiController]
[Route("api/auth")]
public class LogoutController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly JwtOptions _jwtOptions;

    public LogoutController(IMediator mediator, Microsoft.Extensions.Options.IOptions<JwtOptions> jwtOptions)
    {
        _mediator = mediator;
        _jwtOptions = jwtOptions.Value;
    }

    // POST /api/auth/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        Request.Cookies.TryGetValue(_jwtOptions.CookieName, out var refreshToken);

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await _mediator.Send(new RevokeRefreshTokenCommand(refreshToken), cancellationToken);
        }

        Response.Cookies.Delete(_jwtOptions.CookieName);

        return Ok(RequestResponse.Ok("Logged out."));
    }
}
