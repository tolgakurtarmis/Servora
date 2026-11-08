using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Ordering.ListOpenTables;

public sealed class ListOpenTablesQueryHandler
    : IRequestHandler<ListOpenTablesQuery, IReadOnlyList<OpenTableResponse>>
{
    private readonly IRepository<Branch> _branches;
    private readonly ITableSessionRepository _sessions;

    public ListOpenTablesQueryHandler(
        IRepository<Branch> branches,
        ITableSessionRepository sessions)
    {
        _branches = branches;
        _sessions = sessions;
    }

    public async Task<IReadOnlyList<OpenTableResponse>> Handle(
        ListOpenTablesQuery request,
        CancellationToken cancellationToken)
    {
        var branch = await _branches.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null || branch.TenantId != request.TenantId)
        {
            throw new NotFoundException("Branch was not found.");
        }

        var openTables = await _sessions.ListOpenByBranchIdAsync(branch.Id, cancellationToken);
        return [.. openTables.Select(item => new OpenTableResponse(item.TableId, item.TableName, item.SessionId, item.OpenedAt))];
    }
}
