using Servora.Application.Abstractions;

namespace Servora.Application.Tables.UpdateTable;

public sealed record UpdateTableCommand(
    Guid TenantId,
    Guid BranchId,
    Guid TableId,
    string Name,
    bool IsActive) : ICommand<Guid>;

public sealed record UpdateTableRequest(string Name, bool IsActive);
