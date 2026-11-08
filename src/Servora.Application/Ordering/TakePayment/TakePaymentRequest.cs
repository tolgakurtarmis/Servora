using Servora.Domain.Enums;

namespace Servora.Application.Ordering.TakePayment;

public sealed record TakePaymentRequest(PaymentMethod Method, decimal? Amount = null);
