using Servora.Application.Abstractions;

namespace Servora.Application.Ordering.GetTableSession;

public sealed record GetTableSessionQuery(
    Guid BranchId,
    Guid TableSessionId) : IQuery<TableSessionDetailResponse>, IBranchRequest;
