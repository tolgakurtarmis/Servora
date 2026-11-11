using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Enums;

namespace Servora.Application.Ordering.CancelTableSession;

public sealed class CancelTableSessionCommandHandler(
    ITableSessionRepository sessions,
    IOrderLineRepository lines,
    IPaymentRepository payments,
    IBranchRealtime realtime) : IRequestHandler<CancelTableSessionCommand, Guid>
{
    private readonly ITableSessionRepository _sessions = sessions;
    private readonly IOrderLineRepository _lines = lines;
    private readonly IPaymentRepository _payments = payments;
    private readonly IBranchRealtime _realtime = realtime;

    public async Task<Guid> Handle(CancelTableSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessions.GetByIdAsync(request.TableSessionId, cancellationToken);
        if (session is null || session.BranchId != request.BranchId)
        {
            throw new NotFoundException("Table session was not found.");
        }

        if (session.Status != TableSessionStatus.Open)
        {
            throw new BusinessException("Only an open table session can be cancelled.");
        }

        if (await _lines.AnyBySessionIdAsync(session.Id, cancellationToken))
        {
            throw new BusinessException("Cancel all order lines before voiding the session.");
        }

        if (await _payments.AnyBySessionIdAsync(session.Id, cancellationToken))
        {
            throw new BusinessException("A session with payments cannot be voided.");
        }

        session.Status = TableSessionStatus.Cancelled;
        _realtime.TablesChanged(session.BranchId);

        return session.Id;
    }
}
