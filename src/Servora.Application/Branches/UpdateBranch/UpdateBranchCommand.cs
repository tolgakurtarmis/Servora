using Servora.Application.Abstractions;

namespace Servora.Application.Branches.UpdateBranch;

public sealed record UpdateBranchCommand(
    Guid TenantId,
    Guid BranchId,
    string Name,
    bool UsesKitchen,
    bool IsActive) : ICommand<Guid>;

public sealed record UpdateBranchRequest(string Name, bool UsesKitchen, bool IsActive);
