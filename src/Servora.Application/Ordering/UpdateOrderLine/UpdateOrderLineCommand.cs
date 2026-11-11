using Servora.Application.Abstractions;

namespace Servora.Application.Ordering.UpdateOrderLine;

public sealed record UpdateOrderLineCommand(
    Guid BranchId,
    Guid OrderLineId,
    int Quantity,
    string? Note) : ICommand<Guid>, IBranchRequest;
