using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Tables.ListTables;

public sealed class ListTablesQueryHandler
    : IRequestHandler<ListTablesQuery, IReadOnlyList<TableResponse>>
{
    private readonly IRepository<Branch> _branches;
    private readonly ITableRepository _tables;
    private readonly ITableSessionRepository _sessions;

    public ListTablesQueryHandler(
        IRepository<Branch> branches,
        ITableRepository tables,
        ITableSessionRepository sessions)
    {
        _branches = branches;
        _tables = tables;
        _sessions = sessions;
    }

    public async Task<IReadOnlyList<TableResponse>> Handle(
        ListTablesQuery request,
        CancellationToken cancellationToken)
    {
        var branch = await _branches.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null || branch.TenantId != request.TenantId)
        {
            throw new NotFoundException("Branch was not found.");
        }

        var tables = await _tables.ListByBranchIdAsync(branch.Id, cancellationToken);
        var openSessions = await _sessions.ListOpenByBranchIdAsync(branch.Id, cancellationToken);
        var openByTableId = openSessions.ToDictionary(session => session.TableId);

        return tables
            .Select(table =>
            {
                openByTableId.TryGetValue(table.Id, out var openSession);
                return new TableResponse(
                    table.Id,
                    table.Name,
                    openSession is not null,
                    openSession?.SessionId,
                    openSession?.OpenedAt);
            })
            .ToList();
    }
}
