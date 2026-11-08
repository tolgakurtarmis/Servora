using Servora.Application.Abstractions;

namespace Servora.Application.Ordering.ListOpenTables;

public sealed record ListOpenTablesQuery(Guid TenantId, Guid BranchId)
    : IQuery<IReadOnlyList<OpenTableResponse>>;
