using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Tenants.UpdateTenant;

public sealed class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, Guid>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly ITenantRepository _tenants;

    public UpdateTenantCommandHandler(ICurrentTenant currentTenant, ITenantRepository tenants)
    {
        _currentTenant = currentTenant;
        _tenants = tenants;
    }

    public async Task<Guid> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsPlatformAdmin)
        {
            throw new ForbiddenException("Only a platform admin can update tenants.");
        }

        var tenant = await _tenants.GetByIdAsync(request.TenantId, cancellationToken);
        if (tenant is null)
        {
            throw new NotFoundException("Tenant was not found.");
        }

        tenant.Name = request.Name.Trim();
        tenant.IsActive = request.IsActive;
        return tenant.Id;
    }
}
