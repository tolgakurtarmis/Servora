using Servora.Application.Abstractions;
using Servora.Domain.Enums;

namespace Servora.Application.Ordering.TakePayment;

public sealed record TakePaymentCommand(
    Guid TenantId,
    Guid BranchId,
    Guid TableSessionId,
    PaymentMethod Method) : ICommand<TakePaymentResult>, ITenantRequest;

public sealed record TakePaymentResult(Guid Id, decimal Amount);
