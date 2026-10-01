using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticket_API.Data;
using Ticket_API.Models;
using Ticket_API.Services;

namespace Ticket_API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class AuthController(AppDbContext db, TokenService tokens) : ControllerBase
{
    /// <summary>Login agent backoffice dan terima JWT (cek ke PostgreSQL).</summary>
    /// <remarks>Seed: <c>admin@helpdesk.test</c> / <c>password</c>.</remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(
            u => u.Email.ToLower() == email, ct);

        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(ApiResponse<LoginResponse>.Fail(
                "Email atau password salah.",
                [new ApiError("invalid_credentials", "Email atau password salah.")],
                HttpContext.TraceIdentifier));
        }

        var apiUser = user.ToApiUser();
        var (token, expiresAt) = tokens.CreateToken(apiUser);
        return Ok(ApiResponse<LoginResponse>.Ok(
            new LoginResponse(token, expiresAt, apiUser),
            "Login berhasil.",
            HttpContext.TraceIdentifier));
    }

    /// <summary>Registrasi agent baru ke PostgreSQL.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var exists = await db.Users.AnyAsync(u => u.Email.ToLower() == email, ct);
        if (exists)
        {
            return Conflict(ApiResponse<LoginResponse>.Fail(
                "Email sudah terdaftar.",
                [new ApiError("email_taken", "Email sudah terdaftar.", "email")],
                HttpContext.TraceIdentifier));
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Role = "Agent",
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        var apiUser = user.ToApiUser();
        var (token, expiresAt) = tokens.CreateToken(apiUser);
        return CreatedAtAction(nameof(Login), ApiResponse<LoginResponse>.Ok(
            new LoginResponse(token, expiresAt, apiUser),
            "Registrasi berhasil.",
            HttpContext.TraceIdentifier));
    }
}
