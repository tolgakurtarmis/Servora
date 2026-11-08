using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Behaviors;

public sealed class TenantAccessBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentTenant _currentTenant;

    public TenantAccessBehavior(ICurrentTenant currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is ITenantRequest tenantRequest
            && !_currentTenant.IsPlatformAdmin
            && _currentTenant.TenantId != tenantRequest.TenantId)
        {
            throw new ForbiddenException("You can only access your own tenant.");
        }

        return await next();
    }
}
