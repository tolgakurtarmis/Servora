using Servora.Application.Abstractions;

namespace Servora.Application.Ordering.AddOrderRound;

public sealed record OrderLineInput(Guid MenuItemId, int Quantity, string? Note = null);

public sealed record AddOrderRoundCommand(
    Guid TenantId,
    Guid BranchId,
    Guid TableSessionId,
    IReadOnlyList<OrderLineInput> Lines) : ICommand<Guid>, ITenantRequest;
