namespace Servora.Infrastructure.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public int ExpiresMinutes { get; set; } = 15;

    public int RefreshExpiresDays { get; set; } = 7;
}
