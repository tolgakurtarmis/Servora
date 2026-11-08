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

    public UpdateUserCommandHandler(ICurrentTenant currentTenant, UserManager<User> users)
    {
        _currentTenant = currentTenant;
        _users = users;
    }

    public async Task<Guid> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.FindByIdAsync(request.UserId.ToString());
        if (user is null || user.IsDeleted)
        {
            throw new NotFoundException("User was not found.");
        }

        if (!_currentTenant.IsPlatformAdmin && _currentTenant.TenantId != user.TenantId)
        {
            throw new ForbiddenException("You can only update users for your own tenant.");
        }

        user.Name = request.Name.Trim();
        user.IsActive = request.IsActive;

        var updated = await _users.UpdateAsync(user);
        if (!updated.Succeeded)
        {
            throw new BusinessException(string.Join(" ", updated.Errors.Select(error => error.Description)));
        }

        return user.Id;
    }
}
