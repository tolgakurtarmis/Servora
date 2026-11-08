using Servora.Application.Abstractions;

namespace Servora.Application.Catalog.UpdateMenuItem;

public sealed record UpdateMenuItemCommand(
    Guid TenantId,
    Guid BranchId,
    Guid MenuItemId,
    Guid CategoryId,
    string Name,
    decimal Price,
    bool IsActive) : ICommand<Guid>, ITenantRequest;

public sealed record UpdateMenuItemRequest(Guid CategoryId, string Name, decimal Price, bool IsActive);
