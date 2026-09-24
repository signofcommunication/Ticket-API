namespace Ticket_API.Models;

public sealed record LoginRequest(string Email, string Password);

public sealed record ApiUser(string Id, string Name, string Email, string Role);

public sealed record LoginResponse(string Token, DateTime ExpiresAt, ApiUser User);
