using System.ComponentModel.DataAnnotations;

namespace Ticket_API.Models;

public sealed class User
{
    [Key]
    public string Id { get; set; } = $"user-{Guid.NewGuid():N}";

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Role { get; set; } = "Agent";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ApiUser ToApiUser() => new(Id, Name, Email, Role);
}
