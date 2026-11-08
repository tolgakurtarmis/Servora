using Servora.Application.Abstractions;

namespace Servora.Application.Branches.CreateBranch;

public sealed record CreateBranchCommand(Guid TenantId, string Name, bool UsesKitchen) : ICommand<Guid>;
