namespace Servora.Infrastructure.Persistence;

public sealed class SeedOptions
{
    public const string SectionName = "Seed";

    public bool Enabled { get; set; }

    public string PlatformAdminEmail { get; set; } = "admin@servora.local";

    public string PlatformAdminPassword { get; set; } = "Admin123!";
}
