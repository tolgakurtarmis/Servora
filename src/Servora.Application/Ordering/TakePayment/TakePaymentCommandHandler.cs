using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Application.Ordering.TakePayment;

public sealed class TakePaymentCommandHandler : IRequestHandler<TakePaymentCommand, TakePaymentResult>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly ITableSessionRepository _sessions;
    private readonly IOrderLineRepository _lines;
    private readonly IRepository<Payment> _payments;

    public TakePaymentCommandHandler(
        ICurrentTenant currentTenant,
        ITableSessionRepository sessions,
        IOrderLineRepository lines,
        IRepository<Payment> payments)
    {
        _currentTenant = currentTenant;
        _sessions = sessions;
        _lines = lines;
        _payments = payments;
    }

    public async Task<TakePaymentResult> Handle(TakePaymentCommand request, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsPlatformAdmin && _currentTenant.TenantId != request.TenantId)
        {
            throw new ForbiddenException("You can only take payment for your own tenant.");
        }

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

        return new TakePaymentResult(payment.Id, payment.Amount);
    }
}
