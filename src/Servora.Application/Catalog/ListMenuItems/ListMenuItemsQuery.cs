using Servora.Application.Abstractions;

namespace Servora.Application.Catalog.ListMenuItems;

public sealed record ListMenuItemsQuery(Guid TenantId, Guid BranchId)
    : IQuery<IReadOnlyList<MenuItemResponse>>, ITenantRequest;
