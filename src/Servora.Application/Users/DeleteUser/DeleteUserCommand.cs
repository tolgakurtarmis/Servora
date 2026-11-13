using Servora.Application.Abstractions;

namespace Servora.Application.Users.DeleteUser;

public sealed record DeleteUserCommand(Guid UserId) : ICommand<Guid>;
