using Servora.Application.Abstractions;
using Servora.Application.Auth;

namespace Servora.Application.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<AuthResponse>;
