using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Application.Ordering.CancelOrderLine;

public sealed class CancelOrderLineCommandHandler : IRequestHandler<CancelOrderLineCommand, Guid>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly IOrderLineRepository _lines;
    private readonly IRepository<OrderRound> _rounds;
    private readonly ITableSessionRepository _sessions;

    public CancelOrderLineCommandHandler(
        ICurrentTenant currentTenant,
        IOrderLineRepository lines,
        IRepository<OrderRound> rounds,
        ITableSessionRepository sessions)
    {
        _currentTenant = currentTenant;
        _lines = lines;
        _rounds = rounds;
        _sessions = sessions;
    }

    public async Task<Guid> Handle(CancelOrderLineCommand request, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsPlatformAdmin && _currentTenant.TenantId != request.TenantId)
        {
            throw new ForbiddenException("You can only cancel order lines for your own tenant.");
        }

        var line = await _lines.GetByIdAsync(request.OrderLineId, cancellationToken);
        if (line is null || line.TenantId != request.TenantId || line.BranchId != request.BranchId)
        {
            throw new NotFoundException("Order line was not found.");
        }

        var round = await _rounds.GetByIdAsync(line.OrderRoundId, cancellationToken);
        if (round is null)
        {
            throw new NotFoundException("Order round was not found.");
        }

        var session = await _sessions.GetByIdAsync(round.TableSessionId, cancellationToken);
        if (session is null)
        {
            throw new NotFoundException("Table session was not found.");
        }

        if (session.Status != TableSessionStatus.Open)
        {
            throw new BusinessException("Order lines can only be cancelled on an open table session.");
        }

        _lines.Remove(line);
        return line.Id;
    }
}
