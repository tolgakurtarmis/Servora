using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Tables.ListTables;

public sealed class ListTablesQueryHandler
    : IRequestHandler<ListTablesQuery, IReadOnlyList<TableResponse>>
{
    private readonly IAppCache _cache;
    private readonly ICachedBranchReader _branches;
    private readonly ITableRepository _tables;
    private readonly ITableSessionRepository _sessions;

    public ListTablesQueryHandler(
        IAppCache cache,
        ICachedBranchReader branches,
        ITableRepository tables,
        ITableSessionRepository sessions)
    {
        _cache = cache;
        _branches = branches;
        _tables = tables;
        _sessions = sessions;
    }

    public async Task<IReadOnlyList<TableResponse>> Handle(
        ListTablesQuery request,
        CancellationToken cancellationToken)
    {
        var branch = await _branches.GetAsync(request.BranchId, cancellationToken);
        if (branch is null)
        {
            throw new NotFoundException("Branch was not found.");
        }

        var tables = await _cache.GetOrCreateAsync(
            CacheKeys.Tables(branch.Id),
            CacheDurations.Tables,
            async token =>
            {
                var items = await _tables.ListByBranchIdAsync(branch.Id, token);
                return (IReadOnlyList<CachedTable>)items
                    .Select(table => new CachedTable(table.Id, table.Name))
                    .ToList();
            },
            cancellationToken);

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
