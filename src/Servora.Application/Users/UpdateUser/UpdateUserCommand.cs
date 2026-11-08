using Servora.Application.Abstractions;

namespace Servora.Application.Users.UpdateUser;

public sealed record UpdateUserCommand(Guid UserId, string Name, bool IsActive) : ICommand<Guid>;

public sealed record UpdateUserRequest(string Name, bool IsActive);
