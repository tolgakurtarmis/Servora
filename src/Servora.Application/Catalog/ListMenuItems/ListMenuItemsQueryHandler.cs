using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Catalog.ListMenuItems;

public sealed class ListMenuItemsQueryHandler
    : IRequestHandler<ListMenuItemsQuery, IReadOnlyList<MenuItemResponse>>
{
    private readonly IAppCache _cache;
    private readonly ICachedBranchReader _branches;
    private readonly IMenuItemRepository _menuItems;

    public ListMenuItemsQueryHandler(
        IAppCache cache,
        ICachedBranchReader branches,
        IMenuItemRepository menuItems)
    {
        _cache = cache;
        _branches = branches;
        _menuItems = menuItems;
    }

    public async Task<IReadOnlyList<MenuItemResponse>> Handle(
        ListMenuItemsQuery request,
        CancellationToken cancellationToken)
    {
        var branch = await _branches.GetAsync(request.BranchId, cancellationToken);
        if (branch is null || branch.TenantId != request.TenantId)
        {
            throw new NotFoundException("Branch was not found.");
        }

        return await _cache.GetOrCreateAsync(
            CacheKeys.Menus(branch.Id),
            CacheDurations.Menus,
            async token =>
            {
                var items = await _menuItems.ListByBranchIdAsync(branch.Id, token);
                return (IReadOnlyList<MenuItemResponse>)items
                    .Select(item => new MenuItemResponse(item.Id, item.Name, item.Price))
                    .ToList();
            },
            cancellationToken);
    }
}
