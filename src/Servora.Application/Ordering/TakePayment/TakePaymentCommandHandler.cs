using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Application.Ordering.TakePayment;

public sealed class TakePaymentCommandHandler(
    ITableSessionRepository sessions,
    IOrderLineRepository lines,
    IPaymentRepository payments,
    IBranchRealtime realtime) : IRequestHandler<TakePaymentCommand, TakePaymentResult>
{
    private readonly ITableSessionRepository _sessions = sessions;
    private readonly IOrderLineRepository _lines = lines;
    private readonly IPaymentRepository _payments = payments;
    private readonly IBranchRealtime _realtime = realtime;

    public async Task<TakePaymentResult> Handle(TakePaymentCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessions.GetByIdAsync(request.TableSessionId, cancellationToken);
        if (session is null || session.TenantId != request.TenantId || session.BranchId != request.BranchId)
        {
            throw new NotFoundException("Table session was not found.");
        }

        if (session.Status != TableSessionStatus.Open)
        {
            throw new BusinessException("Payment can only be taken for an open table session.");
        }

        var total = await _lines.SumBySessionIdAsync(session.Id, cancellationToken);
        var paid = await _payments.SumBySessionIdAsync(session.Id, cancellationToken);
        var remaining = total - paid;
        if (remaining <= 0)
        {
            throw new BusinessException("This session has no remaining balance to pay.");
        }

        var amount = request.Amount ?? remaining;
        if (amount > remaining)
        {
            throw new BusinessException("Payment amount exceeds the remaining balance.");
        }

        var payment = new Payment
        {
            TenantId = session.TenantId,
            BranchId = session.BranchId,
            TableSessionId = session.Id,
            Amount = amount,
            Method = request.Method
        };

        var newRemaining = remaining - amount;
        var sessionClosed = newRemaining <= 0;
        if (sessionClosed)
        {
            session.Status = TableSessionStatus.Closed;
        }

        await _payments.AddAsync(payment, cancellationToken);
        _realtime.TablesChanged(session.BranchId);
        if (sessionClosed)
        {
            _realtime.KitchenQueueChanged(session.BranchId);
        }

        return new TakePaymentResult(payment.Id, payment.Amount, newRemaining < 0 ? 0 : newRemaining, sessionClosed);
    }
}
