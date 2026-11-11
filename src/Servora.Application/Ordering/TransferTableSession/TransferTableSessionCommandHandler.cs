using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Application.Ordering.TransferTableSession;

public sealed class TransferTableSessionCommandHandler(
    ITableSessionRepository sessions,
    IRepository<Table> tables,
    IBranchRealtime realtime) : IRequestHandler<TransferTableSessionCommand, Guid>
{
    private readonly ITableSessionRepository _sessions = sessions;
    private readonly IRepository<Table> _tables = tables;
    private readonly IBranchRealtime _realtime = realtime;

    public async Task<Guid> Handle(TransferTableSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessions.GetByIdAsync(request.TableSessionId, cancellationToken);
        if (session is null || session.BranchId != request.BranchId)
        {
            throw new NotFoundException("Table session was not found.");
        }

        if (session.Status != TableSessionStatus.Open)
        {
            throw new BusinessException("Only an open table session can be transferred.");
        }

        if (session.TableId == request.TargetTableId)
        {
            throw new BusinessException("Session is already on the target table.");
        }

        var targetTable = await _tables.GetByIdReadOnlyAsync(request.TargetTableId, cancellationToken);
        if (targetTable is null
            || targetTable.BranchId != request.BranchId)
        {
            throw new NotFoundException("Target table was not found.");
        }

        if (await _sessions.HasOpenSessionAsync(targetTable.Id, cancellationToken))
        {
            throw new BusinessException("Target table already has an open session.");
        }

        session.TableId = targetTable.Id;
        _realtime.TablesChanged(session.BranchId);

        return session.Id;
    }
}
