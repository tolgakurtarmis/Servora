using Servora.Application.Abstractions;

namespace Servora.Application.Tables.DeleteTable;

public sealed record DeleteTableCommand(Guid BranchId, Guid TableId) : ICommand<Guid>, IBranchRequest;
