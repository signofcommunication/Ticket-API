using Microsoft.EntityFrameworkCore;
using Ticket_API.Models;
using Ticket_API.Services;

namespace Ticket_API.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).HasMaxLength(320).IsRequired();
            entity.Property(u => u.Name).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Role).HasMaxLength(50).IsRequired();
        });
    }

    public static async Task EnsureCreatedAndSeededAsync(IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync(ct);

        const string adminEmail = "admin@helpdesk.test";
        var exists = await db.Users.AnyAsync(u => u.Email == adminEmail, ct);
        if (!exists)
        {
            db.Users.Add(new User
            {
                Id = "user-1",
                Name = "Nico Abel",
                Email = adminEmail,
                PasswordHash = PasswordHasher.Hash("password"),
                Role = "Agent",
            });
            await db.SaveChangesAsync(ct);
        }
    }
}
