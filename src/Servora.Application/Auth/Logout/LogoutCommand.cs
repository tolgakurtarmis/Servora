using Servora.Application.Abstractions;

namespace Servora.Application.Auth.Logout;

public sealed record LogoutCommand(string RefreshToken) : ICommand<bool>;
