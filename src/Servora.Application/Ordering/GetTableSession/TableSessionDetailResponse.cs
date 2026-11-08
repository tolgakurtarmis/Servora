using Servora.Domain.Enums;

namespace Servora.Application.Ordering.GetTableSession;

public sealed record TableSessionDetailResponse(
    Guid Id,
    Guid TableId,
    TableSessionStatus Status,
    decimal Total,
    IReadOnlyList<TableSessionLineResponse> Lines);

public sealed record TableSessionLineResponse(
    Guid Id,
    Guid OrderRoundId,
    Guid MenuItemId,
    string Name,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal,
    KitchenStatus KitchenStatus);
