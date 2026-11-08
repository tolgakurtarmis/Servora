using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Ordering.ListKitchenQueue;

public sealed class ListKitchenQueueQueryHandler(
    ICachedBranchReader branches,
    IOrderLineRepository lines)
        : IRequestHandler<ListKitchenQueueQuery, IReadOnlyList<KitchenQueueItemResponse>>
{
    private readonly ICachedBranchReader _branches = branches;
    private readonly IOrderLineRepository _lines = lines;

    public async Task<IReadOnlyList<KitchenQueueItemResponse>> Handle(
        ListKitchenQueueQuery request,
        CancellationToken cancellationToken)
    {
        var branch = await _branches.GetAsync(request.BranchId, cancellationToken);
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
                item.Note,
                item.Quantity,
                item.KitchenStatus,
                item.OrderedAt))
        ];
    }
}
