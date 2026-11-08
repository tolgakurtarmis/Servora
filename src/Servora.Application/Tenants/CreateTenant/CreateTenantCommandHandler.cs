using AutoMapper;
using MediatR;
using Servora.Application.Abstractions;
using Servora.Domain.Models;

namespace Servora.Application.Tenants.CreateTenant;

public sealed class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Guid>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Tenant> _tenants;
    private readonly IRepository<Subscription> _subscriptions;

    public CreateTenantCommandHandler(
        IMapper mapper,
        IRepository<Tenant> tenants,
        IRepository<Subscription> subscriptions)
    {
        _mapper = mapper;
        _tenants = tenants;
        _subscriptions = subscriptions;
    }

    public async Task<Guid> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = _mapper.Map<Tenant>(request);
        var subscription = _mapper.Map<Subscription>(request);
        subscription.TenantId = tenant.Id;

        await _tenants.AddAsync(tenant, cancellationToken);
        await _subscriptions.AddAsync(subscription, cancellationToken);

        return tenant.Id;
    }
}
