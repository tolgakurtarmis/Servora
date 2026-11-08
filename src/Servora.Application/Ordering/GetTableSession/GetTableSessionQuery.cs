using Servora.Application.Abstractions;

namespace Servora.Application.Ordering.GetTableSession;

public sealed record GetTableSessionQuery(
    Guid TenantId,
    Guid BranchId,
    Guid TableSessionId) : IQuery<TableSessionDetailResponse>, ITenantRequest;
