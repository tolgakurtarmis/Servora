using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Behaviors;

public sealed class TenantAccessBehavior<TRequest, TResponse>(
    ICurrentTenant currentTenant,
    ICachedBranchReader branches) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (currentTenant.IsPlatformAdmin)
        {
            return await next();
        }

        if (request is IBranchRequest branchRequest)
        {
            var branch = await branches.GetAsync(branchRequest.BranchId, cancellationToken);
            if (branch is not null && currentTenant.TenantId != branch.TenantId)
            {
                throw new ForbiddenException("You can only access your own tenant.");
            }

            return await next();
        }

        if (request is ITenantRequest tenantRequest
            && currentTenant.TenantId != tenantRequest.TenantId)
        {
            throw new ForbiddenException("You can only access your own tenant.");
        }

        return await next();
    }
}
