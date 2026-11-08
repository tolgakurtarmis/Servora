using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Application.Users.CreateUser;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IMapper _mapper;
    private readonly ICurrentTenant _currentTenant;
    private readonly UserManager<User> _users;
    private readonly IRepository<Branch> _branches;
    private readonly IUserBranchRepository _userBranches;

    public CreateUserCommandHandler(
        IMapper mapper,
        ICurrentTenant currentTenant,
        UserManager<User> users,
        IRepository<Branch> branches,
        IUserBranchRepository userBranches)
    {
        _mapper = mapper;
        _currentTenant = currentTenant;
        _users = users;
        _branches = branches;
        _userBranches = userBranches;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var tenantId = ResolveTenantId(request);
        var branchId = await ResolveBranchIdAsync(request, tenantId, cancellationToken);

        var existing = await _users.FindByEmailAsync(request.Email.Trim());
        if (existing is not null)
        {
            throw new BusinessException("A user with this email already exists.");
        }

        var user = _mapper.Map<User>(request);
        user.TenantId = tenantId;

        var created = await _users.CreateAsync(user, request.Password);
        if (!created.Succeeded)
        {
            throw new BusinessException(string.Join(" ", created.Errors.Select(error => error.Description)));
        }

        var roleAssigned = await _users.AddToRoleAsync(user, request.Role.ToString());
        if (!roleAssigned.Succeeded)
        {
            throw new BusinessException(string.Join(" ", roleAssigned.Errors.Select(error => error.Description)));
        }

        if (branchId is not null)
        {
            await _userBranches.AddAsync(new UserBranch
            {
                UserId = user.Id,
                TenantId = tenantId!.Value,
                BranchId = branchId.Value
            }, cancellationToken);
        }

        return user.Id;
    }

    private Guid? ResolveTenantId(CreateUserCommand request)
    {
        if (request.Role == Role.PlatformAdmin)
        {
            if (!_currentTenant.IsPlatformAdmin)
            {
                throw new ForbiddenException("Only a platform admin can create a platform admin.");
            }

            return null;
        }

        if (_currentTenant.IsPlatformAdmin)
        {
            return request.TenantId
                ?? throw new BusinessException("Tenant is required for this role.");
        }

        if (_currentTenant.TenantId is null)
        {
            throw new ForbiddenException("The current user is not assigned to a tenant.");
        }

        if (request.TenantId is not null && request.TenantId != _currentTenant.TenantId)
        {
            throw new ForbiddenException("You can only create users for your own tenant.");
        }

        return _currentTenant.TenantId;
    }

    private async Task<Guid?> ResolveBranchIdAsync(
        CreateUserCommand request,
        Guid? tenantId,
        CancellationToken cancellationToken)
    {
        if (request.BranchId is null)
        {
            return null;
        }

        var branch = await _branches.GetByIdAsync(request.BranchId.Value, cancellationToken);
        if (branch is null || branch.TenantId != tenantId)
        {
            throw new NotFoundException("Branch was not found.");
        }

        return branch.Id;
    }
}
