using MediatR;
using Microsoft.AspNetCore.Identity;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Users.UpdateUser;

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Guid>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly UserManager<User> _users;
    private readonly IUserBranchRepository _userBranches;

    public UpdateUserCommandHandler(
        ICurrentTenant currentTenant,
        UserManager<User> users,
        IUserBranchRepository userBranches)
    {
        _currentTenant = currentTenant;
        _users = users;
        _userBranches = userBranches;
    }

    public async Task<Guid> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.FindByIdAsync(request.UserId.ToString());
        if (user is null || user.IsDeleted)
        {
            throw new NotFoundException("User was not found.");
        }

        await UserManagementRules.EnsureCanManageUserAsync(
            _currentTenant,
            _userBranches,
            user,
            cancellationToken);

        UserManagementRules.EnsureCanAssignRole(_currentTenant, request.Role);

        user.Name = request.Name.Trim();
        user.IsActive = request.IsActive;

        if (user.Role != request.Role)
        {
            var currentRoles = await _users.GetRolesAsync(user);
            if (currentRoles.Count > 0)
            {
                var removed = await _users.RemoveFromRolesAsync(user, currentRoles);
                if (!removed.Succeeded)
                {
                    throw new BusinessException(string.Join(" ", removed.Errors.Select(error => error.Description)));
                }
            }

            var assigned = await _users.AddToRoleAsync(user, request.Role.ToString());
            if (!assigned.Succeeded)
            {
                throw new BusinessException(string.Join(" ", assigned.Errors.Select(error => error.Description)));
            }

            user.Role = request.Role;
        }

        var updated = await _users.UpdateAsync(user);
        if (!updated.Succeeded)
        {
            throw new BusinessException(string.Join(" ", updated.Errors.Select(error => error.Description)));
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var resetToken = await _users.GeneratePasswordResetTokenAsync(user);
            var passwordUpdated = await _users.ResetPasswordAsync(user, resetToken, request.Password);
            if (!passwordUpdated.Succeeded)
            {
                throw new BusinessException(string.Join(" ", passwordUpdated.Errors.Select(error => error.Description)));
            }
        }

        return user.Id;
    }
}
