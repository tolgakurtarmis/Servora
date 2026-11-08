namespace Servora.Application.Auth.Login;

public sealed record LoginResponse(string AccessToken, DateTime ExpiresAt);
