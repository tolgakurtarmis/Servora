using Servora.Application.Abstractions;
using Servora.Domain.Enums;

namespace Servora.Application.Users.ListUsers;

public sealed record ListUsersQuery(Guid? TenantId) : IQuery<IReadOnlyList<UserResponse>>;

public sealed record UserResponse(
    Guid Id,
    string Name,
    string UserName,
    Role Role,
    Guid? TenantId,
    bool IsActive);
