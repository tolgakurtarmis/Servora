using Servora.Application.Abstractions;

namespace Servora.Application.Catalog.CreateMenuItem;

public sealed record CreateMenuItemCommand(Guid TenantId, Guid BranchId, string Name, decimal Price) : ICommand<Guid>;
