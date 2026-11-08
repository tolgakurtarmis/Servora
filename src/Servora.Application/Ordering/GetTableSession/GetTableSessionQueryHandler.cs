using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Ordering.GetTableSession;

public sealed class GetTableSessionQueryHandler : IRequestHandler<GetTableSessionQuery, TableSessionDetailResponse>
{
    private readonly ITableSessionRepository _sessions;
    private readonly IOrderLineRepository _lines;

    public GetTableSessionQueryHandler(
        ITableSessionRepository sessions,
        IOrderLineRepository lines)
    {
        _sessions = sessions;
        _lines = lines;
    }

    public async Task<TableSessionDetailResponse> Handle(
        GetTableSessionQuery request,
        CancellationToken cancellationToken)
    {
        var session = await _sessions.GetByIdAsync(request.TableSessionId, cancellationToken);
        if (session is null || session.TenantId != request.TenantId || session.BranchId != request.BranchId)
        {
            throw new NotFoundException("Table session was not found.");
        }

        var lines = await _lines.ListBySessionIdAsync(session.Id, cancellationToken);
        var lineResponses = lines
            .Select(line => new TableSessionLineResponse(
                line.Id,
                line.OrderRoundId,
                line.MenuItemId,
                line.Name,
                line.UnitPrice,
                line.Quantity,
                line.UnitPrice * line.Quantity,
                line.KitchenStatus))
            .ToList();

        return new TableSessionDetailResponse(
            session.Id,
            session.TableId,
            session.Status,
            lineResponses.Sum(line => line.LineTotal),
            lineResponses);
    }
}
