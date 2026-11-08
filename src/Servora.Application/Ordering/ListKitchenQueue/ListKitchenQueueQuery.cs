using Servora.Application.Abstractions;

namespace Servora.Application.Ordering.ListKitchenQueue;

public sealed record ListKitchenQueueQuery(Guid TenantId, Guid BranchId)
    : IQuery<IReadOnlyList<KitchenQueueItemResponse>>;
