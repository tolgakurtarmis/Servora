using Servora.Application.Abstractions;

namespace Servora.Application.Ordering.TransferTableSession;

public sealed record TransferTableSessionCommand(
    Guid TenantId,
    Guid BranchId,
    Guid TableSessionId,
    Guid TargetTableId) : ICommand<Guid>, ITenantRequest;
