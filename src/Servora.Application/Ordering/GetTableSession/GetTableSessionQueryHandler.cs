using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Ordering.GetTableSession;

public sealed class GetTableSessionQueryHandler(
    ITableSessionRepository sessions,
    IOrderLineRepository lines,
    IPaymentRepository payments) : IRequestHandler<GetTableSessionQuery, TableSessionDetailResponse>
{
    private readonly ITableSessionRepository _sessions = sessions;
    private readonly IOrderLineRepository _lines = lines;
    private readonly IPaymentRepository _payments = payments;

    public async Task<TableSessionDetailResponse> Handle(
        GetTableSessionQuery request,
        CancellationToken cancellationToken)
    {
        var session = await _sessions.GetByIdReadOnlyAsync(request.TableSessionId, cancellationToken);
        if (session is null || session.BranchId != request.BranchId)
        {
            throw new NotFoundException("Table session was not found.");
        }

        var orderLines = await _lines.ListBySessionIdAsync(session.Id, cancellationToken);
        var lineResponses = orderLines
            .Select(line => new TableSessionLineResponse(
                line.Id,
                line.OrderRoundId,
                line.MenuItemId,
                line.Name,
                line.Note,
                line.UnitPrice,
                line.Quantity,
                line.UnitPrice * line.Quantity,
                line.KitchenStatus))
            .ToList();

        var total = lineResponses.Sum(line => line.LineTotal);
        var paid = await _payments.SumBySessionIdAsync(session.Id, cancellationToken);
        var remaining = total - paid;
        if (remaining < 0)
        {
            remaining = 0;
        }

        return new TableSessionDetailResponse(
            session.Id,
            session.TableId,
            session.Status,
            total,
            paid,
            remaining,
            lineResponses);
    }
}
