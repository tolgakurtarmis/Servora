using Servora.Application.Abstractions;
using Servora.Application.Auth;

namespace Servora.Application.Auth.Refresh;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<AuthResponse>;
