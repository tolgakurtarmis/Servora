namespace Servora.Application.Branches.CreateBranch;

public sealed record CreateBranchRequest(string Name, bool? UsesKitchen, Guid? TenantId = null);
