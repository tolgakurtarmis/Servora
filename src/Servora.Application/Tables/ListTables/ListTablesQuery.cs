using Servora.Application.Abstractions;

namespace Servora.Application.Tables.ListTables;

public sealed record ListTablesQuery(Guid TenantId, Guid BranchId)
    : IQuery<IReadOnlyList<TableResponse>>;
