using Servora.Application.Abstractions;
using Servora.Domain.Enums;

namespace Servora.Application.Users.CreateUser;

public sealed record CreateUserCommand(
    string Name,
    string UserName,
    string Password,
    Role Role,
    Guid? TenantId,
    Guid? BranchId) : ICommand<Guid>;
