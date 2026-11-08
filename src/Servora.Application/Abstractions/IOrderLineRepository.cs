using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Application.Abstractions;

public interface IOrderLineRepository : IRepository<OrderLine>
{
    Task<decimal> SumBySessionIdAsync(Guid tableSessionId, CancellationToken cancellationToken);

    Task<bool> AnyBySessionIdAsync(Guid tableSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<OrderLine>> ListBySessionIdAsync(Guid tableSessionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<KitchenQueueItemInfo>> ListKitchenQueueByBranchIdAsync(
        Guid branchId,
        CancellationToken cancellationToken);
}

public sealed record KitchenQueueItemInfo(
    Guid OrderLineId,
    Guid TableId,
    string TableName,
    Guid SessionId,
    string Name,
    string Note,
    int Quantity,
    KitchenStatus KitchenStatus,
    DateTime OrderedAt);
