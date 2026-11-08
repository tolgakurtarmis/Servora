using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Application.Ordering.TakePayment;

public sealed class TakePaymentCommandHandler(
    ITableSessionRepository sessions,
    IOrderLineRepository lines,
    IRepository<Payment> payments,
    IBranchRealtime realtime) : IRequestHandler<TakePaymentCommand, TakePaymentResult>
{
    private readonly ITableSessionRepository _sessions = sessions;
    private readonly IOrderLineRepository _lines = lines;
    private readonly IRepository<Payment> _payments = payments;
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

        var amount = await _lines.SumBySessionIdAsync(session.Id, cancellationToken);
        var payment = new Payment
        {
            TenantId = session.TenantId,
            BranchId = session.BranchId,
            TableSessionId = session.Id,
            Amount = amount,
            Method = request.Method
        };

        session.Status = TableSessionStatus.Closed;
        await _payments.AddAsync(payment, cancellationToken);
        _realtime.TablesChanged(session.BranchId);
        _realtime.KitchenQueueChanged(session.BranchId);

        return new TakePaymentResult(payment.Id, payment.Amount);
    }
}
