namespace Servora.Application.Auth;

public sealed record AuthResponse(
    string AccessToken,
    DateTime ExpiresAt,
    string RefreshToken,
    DateTime RefreshExpiresAt);
