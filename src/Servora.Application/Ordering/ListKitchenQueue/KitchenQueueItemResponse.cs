using Servora.Domain.Enums;

namespace Servora.Application.Ordering.ListKitchenQueue;

public sealed record KitchenQueueItemResponse(
    Guid OrderLineId,
    Guid TableId,
    string TableName,
    Guid SessionId,
    string Name,
    string Note,
    int Quantity,
    KitchenStatus KitchenStatus,
    DateTime OrderedAt);
