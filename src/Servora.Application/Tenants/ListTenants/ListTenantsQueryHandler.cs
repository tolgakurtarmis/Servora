using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Tenants.ListTenants;

public sealed class ListTenantsQueryHandler : IRequestHandler<ListTenantsQuery, IReadOnlyList<TenantResponse>>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly ITenantRepository _tenants;

    public ListTenantsQueryHandler(ICurrentTenant currentTenant, ITenantRepository tenants)
    {
        _currentTenant = currentTenant;
        _tenants = tenants;
    }

    public async Task<IReadOnlyList<TenantResponse>> Handle(ListTenantsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsPlatformAdmin)
        {
            throw new ForbiddenException("Only a platform admin can list tenants.");
        }

        var tenants = await _tenants.ListAsync(cancellationToken);
        return tenants
            .Select(tenant => new TenantResponse(tenant.Id, tenant.Name, tenant.IsActive))
            .ToList();
    }
}
