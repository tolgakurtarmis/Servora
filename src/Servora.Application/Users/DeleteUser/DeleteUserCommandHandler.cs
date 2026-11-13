using MediatR;
using Microsoft.AspNetCore.Identity;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Users.DeleteUser;

public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Guid>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly UserManager<User> _users;
    private readonly IUserBranchRepository _userBranches;

    public DeleteUserCommandHandler(
        ICurrentTenant currentTenant,
        UserManager<User> users,
        IUserBranchRepository userBranches)
    {
        _currentTenant = currentTenant;
        _users = users;
        _userBranches = userBranches;
    }

    public async Task<Guid> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        if (_currentTenant.UserId == request.UserId)
        {
            throw new BusinessException("You cannot delete your own account.");
        }

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

        user.IsDeleted = true;
        user.IsActive = false;

        var updated = await _users.UpdateAsync(user);
        if (!updated.Succeeded)
        {
            throw new BusinessException(string.Join(" ", updated.Errors.Select(error => error.Description)));
        }

        return user.Id;
    }
}
