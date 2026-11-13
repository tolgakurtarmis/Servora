using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Application.Users;

internal static class UserManagementRules
{
    public static bool IsManager(ICurrentTenant currentTenant) =>
        currentTenant.Role == nameof(Role.Manager);

    public static void EnsureCanAssignRole(ICurrentTenant currentTenant, Role role)
    {
        if (role == Role.PlatformAdmin && !currentTenant.IsPlatformAdmin)
        {
            throw new ForbiddenException("Only a platform admin can assign the platform admin role.");
        }

        if (IsManager(currentTenant)
            && role is Role.PlatformAdmin or Role.Owner)
        {
            throw new ForbiddenException("Branch managers can only assign operational roles.");
        }
    }

    public static async Task EnsureCanManageUserAsync(
        ICurrentTenant currentTenant,
        IUserBranchRepository userBranches,
        User user,
        CancellationToken cancellationToken)
    {
        if (currentTenant.IsPlatformAdmin)
        {
            return;
        }

        if (currentTenant.TenantId is null || currentTenant.TenantId != user.TenantId)
        {
            throw new ForbiddenException("You can only manage users for your own tenant.");
        }

        if (!IsManager(currentTenant))
        {
            return;
        }

        if (user.Role is Role.PlatformAdmin or Role.Owner)
        {
            throw new ForbiddenException("Branch managers cannot manage this user.");
        }

        if (currentTenant.BranchId is null)
        {
            throw new ForbiddenException("The current user is not assigned to a branch.");
        }

        var belongsToBranch = await userBranches.ExistsAsync(
            user.Id,
            currentTenant.BranchId.Value,
            cancellationToken);

        if (!belongsToBranch)
        {
            throw new ForbiddenException("You can only manage users for your own branch.");
        }
    }
}
