using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Ordering.ListKitchenQueue;

public sealed class ListKitchenQueueQueryHandler
    : IRequestHandler<ListKitchenQueueQuery, IReadOnlyList<KitchenQueueItemResponse>>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly IRepository<Branch> _branches;
    private readonly IOrderLineRepository _lines;

    public ListKitchenQueueQueryHandler(
        ICurrentTenant currentTenant,
        IRepository<Branch> branches,
        IOrderLineRepository lines)
    {
        _currentTenant = currentTenant;
        _branches = branches;
        _lines = lines;
    }

    public async Task<IReadOnlyList<KitchenQueueItemResponse>> Handle(
        ListKitchenQueueQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsPlatformAdmin && _currentTenant.TenantId != request.TenantId)
        {
            throw new ForbiddenException("You can only view the kitchen queue for your own tenant.");
        }

        var branch = await _branches.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null || branch.TenantId != request.TenantId)
        {
            throw new NotFoundException("Branch was not found.");
        }

        if (!branch.UsesKitchen)
        {
            throw new BusinessException("This branch does not use a kitchen workflow.");
        }

        var queue = await _lines.ListKitchenQueueByBranchIdAsync(branch.Id, cancellationToken);
        return
        [
            .. queue.Select(item => new KitchenQueueItemResponse(
                item.OrderLineId,
                item.TableId,
                item.TableName,
                item.SessionId,
                item.Name,
                item.Quantity,
                item.KitchenStatus,
                item.OrderedAt))
        ];
    }
}
