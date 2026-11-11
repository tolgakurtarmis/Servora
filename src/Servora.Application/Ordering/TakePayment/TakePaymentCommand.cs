using Servora.Application.Abstractions;
using Servora.Domain.Enums;

namespace Servora.Application.Ordering.TakePayment;

public sealed record TakePaymentCommand(
    Guid BranchId,
    Guid TableSessionId,
    PaymentMethod Method,
    decimal? Amount = null) : ICommand<TakePaymentResult>, IBranchRequest;

public sealed record TakePaymentResult(
    Guid Id,
    decimal Amount,
    decimal Remaining,
    bool SessionClosed);
