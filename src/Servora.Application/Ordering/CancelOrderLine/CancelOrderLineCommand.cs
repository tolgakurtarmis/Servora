using Servora.Application.Abstractions;

namespace Servora.Application.Ordering.CancelOrderLine;

public sealed record CancelOrderLineCommand(
    Guid TenantId,
    Guid BranchId,
    Guid OrderLineId) : ICommand<Guid>;
