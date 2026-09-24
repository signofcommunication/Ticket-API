using Microsoft.AspNetCore.Mvc;
using Ticket_API.Data;
using Ticket_API.Models;
using Ticket_API.Services;

namespace Ticket_API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class AuthController(TokenService tokens) : ControllerBase
{
    /// <summary>Login agent backoffice dan terima JWT.</summary>
    /// <remarks>Demo: <c>admin@helpdesk.test</c> / <c>password</c>.</remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        var match = InMemoryStore.Users.FirstOrDefault(u =>
            string.Equals(u.User.Email, request.Email?.Trim(), StringComparison.OrdinalIgnoreCase));

        if (match.User is null || match.Password != request.Password)
        {
            return Unauthorized(new { message = "Email atau password salah." });
        }

        var (token, expiresAt) = tokens.CreateToken(match.User);
        return Ok(new LoginResponse(token, expiresAt, match.User));
    }
}
