using Servora.Application.Abstractions;

namespace Servora.Application.Tables.UpdateTable;

public sealed record UpdateTableCommand(
    Guid BranchId,
    Guid TableId,
    string Name,
    bool IsActive) : ICommand<Guid>, IBranchRequest;

public sealed record UpdateTableRequest(string Name, bool IsActive);
