namespace Servora.Application.Ordering.AddOrderRound;

public sealed record AddOrderRoundRequest(IReadOnlyList<OrderLineInput> Lines);
