using Servora.Application.Abstractions;

namespace Servora.Application.Ordering.ListOpenTables;

public sealed record ListOpenTablesQuery(Guid BranchId)
    : IQuery<IReadOnlyList<OpenTableResponse>>, IBranchRequest;
