namespace Servora.Infrastructure.Persistence;

public sealed class SeedOptions
{
    public const string SectionName = "Seed";

    public bool Enabled { get; set; }

    public string PlatformAdminUserName { get; set; } = "admin";

    public string PlatformAdminPassword { get; set; } = "Admin123!";
}
