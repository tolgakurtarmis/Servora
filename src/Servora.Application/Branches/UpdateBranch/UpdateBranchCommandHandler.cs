using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Branches.UpdateBranch;

public sealed class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, Guid>
{
    private readonly IBranchRepository _branches;

    public UpdateBranchCommandHandler(IBranchRepository branches)
    {
        _branches = branches;
    }

    public async Task<Guid> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
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
