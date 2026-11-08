namespace Servora.Application.Ordering.UpdateOrderLine;

public sealed record UpdateOrderLineRequest(int Quantity, string? Note = null);
