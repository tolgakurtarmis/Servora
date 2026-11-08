using Servora.Domain.Entities;

namespace Servora.Domain.Models;

public sealed class RefreshToken : Entity
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public bool IsUsable => RevokedAt is null && ExpiresAt > DateTime.UtcNow;
}
