using Servora.Application.Abstractions;

namespace Servora.Application.Catalog.UpdateMenuItem;

public sealed record UpdateMenuItemCommand(
    Guid TenantId,
    Guid BranchId,
    Guid MenuItemId,
    string Name,
    decimal Price,
    bool IsActive) : ICommand<Guid>, ITenantRequest;

public sealed record UpdateMenuItemRequest(string Name, decimal Price, bool IsActive);
