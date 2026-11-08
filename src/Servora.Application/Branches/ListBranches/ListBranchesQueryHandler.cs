using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Branches.ListBranches;

public sealed class ListBranchesQueryHandler(
    ITenantRepository tenants,
    IBranchRepository branches) : IRequestHandler<ListBranchesQuery, IReadOnlyList<BranchResponse>>
{
    private readonly ITenantRepository _tenants = tenants;
    private readonly IBranchRepository _branches = branches;

    public async Task<IReadOnlyList<BranchResponse>> Handle(
        ListBranchesQuery request,
        CancellationToken cancellationToken)
    {
        var tenant = await _tenants.GetByIdReadOnlyAsync(request.TenantId, cancellationToken) ?? throw new NotFoundException("Tenant was not found.");

        var getBranches = await _branches.ListByTenantIdAsync(tenant.Id, cancellationToken);
        return [.. getBranches.Select(branch => new BranchResponse(branch.Id, branch.Name, branch.UsesKitchen, branch.IsActive))];
    }
}
