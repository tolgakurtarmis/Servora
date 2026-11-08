using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Branches.UpdateBranch;

public sealed class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, Guid>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly IBranchRepository _branches;

    public UpdateBranchCommandHandler(ICurrentTenant currentTenant, IBranchRepository branches)
    {
        _currentTenant = currentTenant;
        _branches = branches;
    }

    public async Task<Guid> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsPlatformAdmin && _currentTenant.TenantId != request.TenantId)
        {
            throw new ForbiddenException("You can only update branches for your own tenant.");
        }

        var branch = await _branches.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null || branch.TenantId != request.TenantId)
        {
            throw new NotFoundException("Branch was not found.");
        }

        branch.Name = request.Name.Trim();
        branch.UsesKitchen = request.UsesKitchen;
        branch.IsActive = request.IsActive;
        return branch.Id;
    }
}
