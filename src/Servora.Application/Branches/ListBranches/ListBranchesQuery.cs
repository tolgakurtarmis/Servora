using Servora.Application.Abstractions;

namespace Servora.Application.Branches.ListBranches;

public sealed record ListBranchesQuery(Guid TenantId) : IQuery<IReadOnlyList<BranchResponse>>;

public sealed record BranchResponse(Guid Id, string Name, bool UsesKitchen, bool IsActive);
