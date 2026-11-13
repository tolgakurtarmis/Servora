using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Users.ListUsers;

public sealed class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, IReadOnlyList<UserResponse>>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly IUserQueryRepository _users;

    public ListUsersQueryHandler(ICurrentTenant currentTenant, IUserQueryRepository users)
    {
        _currentTenant = currentTenant;
        _users = users;
    }

    public async Task<IReadOnlyList<UserResponse>> Handle(
        ListUsersQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsPlatformAdmin && _currentTenant.TenantId is null)
        {
            throw new ForbiddenException("The current user is not assigned to a tenant.");
        }

        if (!_currentTenant.IsPlatformAdmin && request.TenantId is not null && request.TenantId != _currentTenant.TenantId)
        {
            throw new ForbiddenException("You can only list users for your own tenant.");
        }

        var tenantId = _currentTenant.IsPlatformAdmin ? request.TenantId : _currentTenant.TenantId;
        if (!_currentTenant.IsPlatformAdmin && tenantId is null)
        {
            throw new ForbiddenException("Tenant is required.");
        }

        Guid? branchId = null;
        if (UserManagementRules.IsManager(_currentTenant))
        {
            branchId = _currentTenant.BranchId
                ?? throw new ForbiddenException("The current user is not assigned to a branch.");
        }

        var users = await _users.ListAsync(tenantId, branchId, cancellationToken);
        return users
            .Select(user => new UserResponse(
                user.Id,
                user.Name,
                user.UserName ?? string.Empty,
                user.Role,
                user.TenantId,
                user.IsActive))
            .ToList();
    }
}
