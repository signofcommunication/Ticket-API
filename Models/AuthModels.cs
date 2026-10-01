using System.ComponentModel.DataAnnotations;

namespace Ticket_API.Models;

public sealed record LoginRequest(
    [Required(ErrorMessage = "Email wajib diisi.")]
    [EmailAddress(ErrorMessage = "Format email tidak valid.")]
    string Email,

    [Required(ErrorMessage = "Password wajib diisi.")]
    string Password);

public sealed record ApiUser(string Id, string Name, string Email, string Role);

public sealed record LoginResponse(string Token, DateTime ExpiresAt, ApiUser User);

public sealed record RegisterRequest(
    [Required(ErrorMessage = "Nama wajib diisi.")]
    [MinLength(2, ErrorMessage = "Nama minimal 2 karakter.")]
    string Name,

    [Required(ErrorMessage = "Email wajib diisi.")]
    [EmailAddress(ErrorMessage = "Format email tidak valid.")]
    string Email,

    [Required(ErrorMessage = "Password wajib diisi.")]
    [MinLength(6, ErrorMessage = "Password minimal 6 karakter.")]
    string Password);
