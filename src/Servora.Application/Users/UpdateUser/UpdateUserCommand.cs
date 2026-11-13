using Servora.Application.Abstractions;
using Servora.Domain.Enums;

namespace Servora.Application.Users.UpdateUser;

public sealed record UpdateUserCommand(
    Guid UserId,
    string Name,
    bool IsActive,
    Role Role,
    string? Password) : ICommand<Guid>;

public sealed record UpdateUserRequest(
    string Name,
    bool IsActive,
    Role Role,
    string? Password);
