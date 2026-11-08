using Servora.Application.Abstractions;

namespace Servora.Application.Tables.CreateTable;

public sealed record CreateTableCommand(Guid TenantId, Guid BranchId, string Name) : ICommand<Guid>;
