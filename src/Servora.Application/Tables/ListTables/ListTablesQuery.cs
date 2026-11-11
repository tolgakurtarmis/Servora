using Servora.Application.Abstractions;

namespace Servora.Application.Tables.ListTables;

public sealed record ListTablesQuery(Guid BranchId)
    : IQuery<IReadOnlyList<TableResponse>>, IBranchRequest;
