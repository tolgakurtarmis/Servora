using Servora.Application.Abstractions;

namespace Servora.Application.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;
