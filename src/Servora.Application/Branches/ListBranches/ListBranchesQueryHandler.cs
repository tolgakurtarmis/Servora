using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Branches.ListBranches;

public sealed class ListBranchesQueryHandler : IRequestHandler<ListBranchesQuery, IReadOnlyList<BranchResponse>>
{
    private readonly ITenantRepository _tenants;
    private readonly IBranchRepository _branches;

    public ListBranchesQueryHandler(
        ITenantRepository tenants,
        IBranchRepository branches)
    {
        _tenants = tenants;
        _branches = branches;
    }

    public async Task<IReadOnlyList<BranchResponse>> Handle(
        ListBranchesQuery request,
        CancellationToken cancellationToken)
    {
        var tenant = await _tenants.GetByIdReadOnlyAsync(request.TenantId, cancellationToken);
        if (tenant is null)
        {
            throw new NotFoundException("Tenant was not found.");
        }

        var branches = await _branches.ListByTenantIdAsync(tenant.Id, cancellationToken);
        return branches
            .Select(branch => new BranchResponse(branch.Id, branch.Name, branch.UsesKitchen, branch.IsActive))
            .ToList();
    }
}
