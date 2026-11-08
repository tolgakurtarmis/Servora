using AutoMapper;
using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Application.Branches.CreateBranch;

public sealed class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, Guid>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Tenant> _tenants;
    private readonly ISubscriptionRepository _subscriptions;
    private readonly IBranchRepository _branches;

    public CreateBranchCommandHandler(
        IMapper mapper,
        IRepository<Tenant> tenants,
        ISubscriptionRepository subscriptions,
        IBranchRepository branches)
    {
        _mapper = mapper;
        _tenants = tenants;
        _subscriptions = subscriptions;
        _branches = branches;
    }

    public async Task<Guid> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _tenants.GetByIdReadOnlyAsync(request.TenantId, cancellationToken);
        if (tenant is null)
        {
            throw new NotFoundException("Tenant was not found.");
        }

        var subscription = await _subscriptions.GetByTenantIdAsync(request.TenantId, cancellationToken);
        if (subscription is null)
        {
            throw new NotFoundException("Subscription was not found.");
        }

        if (subscription.Status is SubscriptionStatus.PastDue or SubscriptionStatus.Suspended)
        {
            throw new BusinessException("A new branch cannot be created while the subscription is past due or suspended.");
        }

        var branchCount = await _branches.CountByTenantIdAsync(request.TenantId, cancellationToken);
        if (branchCount >= subscription.MaxBranches)
        {
            throw new BusinessException("The branch limit for this subscription has been reached.");
        }

        var branch = _mapper.Map<Branch>(request);
        await _branches.AddAsync(branch, cancellationToken);

        return branch.Id;
    }
}
