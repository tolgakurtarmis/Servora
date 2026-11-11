using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Application.Ordering.UpdateOrderLine;

public sealed class UpdateOrderLineCommandHandler(
    IOrderLineRepository lines,
    IRepository<OrderRound> rounds,
    ITableSessionRepository sessions,
    IBranchRealtime realtime) : IRequestHandler<UpdateOrderLineCommand, Guid>
{
    private readonly IOrderLineRepository _lines = lines;
    private readonly IRepository<OrderRound> _rounds = rounds;
    private readonly ITableSessionRepository _sessions = sessions;
    private readonly IBranchRealtime _realtime = realtime;

    public async Task<Guid> Handle(UpdateOrderLineCommand request, CancellationToken cancellationToken)
    {
        var line = await _lines.GetByIdAsync(request.OrderLineId, cancellationToken);
        if (line is null || line.BranchId != request.BranchId)
        {
            throw new NotFoundException("Order line was not found.");
        }

        var round = await _rounds.GetByIdReadOnlyAsync(line.OrderRoundId, cancellationToken)
            ?? throw new NotFoundException("Order round was not found.");

        var session = await _sessions.GetByIdReadOnlyAsync(round.TableSessionId, cancellationToken)
            ?? throw new NotFoundException("Table session was not found.");

        if (session.Status != TableSessionStatus.Open)
        {
            throw new BusinessException("Order lines can only be updated on an open table session.");
        }

        if (line.KitchenStatus is KitchenStatus.Preparing or KitchenStatus.Served)
        {
            throw new BusinessException("Order lines that are preparing or already served cannot be updated.");
        }

        line.Quantity = request.Quantity;
        line.Note = string.IsNullOrWhiteSpace(request.Note) ? string.Empty : request.Note.Trim();
        _realtime.KitchenQueueChanged(line.BranchId);

        return line.Id;
    }
}
