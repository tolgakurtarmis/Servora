using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Ordering.ListOpenTables;

public sealed class ListOpenTablesQueryHandler(
    ICachedBranchReader branches,
    ITableSessionRepository sessions)
        : IRequestHandler<ListOpenTablesQuery, IReadOnlyList<OpenTableResponse>>
{
    private readonly ICachedBranchReader _branches = branches;
    private readonly ITableSessionRepository _sessions = sessions;

    public async Task<IReadOnlyList<OpenTableResponse>> Handle(
        ListOpenTablesQuery request,
        CancellationToken cancellationToken)
    {
        var branch = await _branches.GetAsync(request.BranchId, cancellationToken);
        if (branch is null)
        {
            throw new NotFoundException("Branch was not found.");
        }

        var openTables = await _sessions.ListOpenByBranchIdAsync(branch.Id, cancellationToken);
        return [.. openTables.Select(item => new OpenTableResponse(item.TableId, item.TableName, item.SessionId, item.OpenedAt))];
    }
}
