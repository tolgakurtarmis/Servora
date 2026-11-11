using Servora.Application.Abstractions;

namespace Servora.Application.Ordering.ListKitchenQueue;

public sealed record ListKitchenQueueQuery(Guid BranchId)
    : IQuery<IReadOnlyList<KitchenQueueItemResponse>>, IBranchRequest;
