using Servora.Application.Abstractions;
using Servora.Application.Auth;

namespace Servora.Application.Auth.Login;

public sealed record LoginCommand(string UserName, string Password) : ICommand<AuthResponse>;
