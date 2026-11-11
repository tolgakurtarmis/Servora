using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Catalog.ListMenuItems;

public sealed class ListMenuItemsQueryHandler(
    IAppCache cache,
    ICachedBranchReader branches,
    IMenuItemRepository menuItems)
    : IRequestHandler<ListMenuItemsQuery, IReadOnlyList<MenuItemResponse>>
{
    public async Task<IReadOnlyList<MenuItemResponse>> Handle(
        ListMenuItemsQuery request,
        CancellationToken cancellationToken)
    {
        var branch = await branches.GetAsync(request.BranchId, cancellationToken);
        if (branch is null)
        {
            throw new NotFoundException("Branch was not found.");
        }

        return await cache.GetOrCreateAsync(
            CacheKeys.Menus(branch.Id),
            CacheDurations.Menus,
            async token =>
            {
                var items = await menuItems.ListByBranchIdAsync(branch.Id, token);
                return (IReadOnlyList<MenuItemResponse>)items
                    .Select(item => new MenuItemResponse(
                        item.Id,
                        item.CategoryId,
                        item.Category.Name,
                        item.Category.SortOrder,
                        item.Name,
                        item.Price))
                    .ToList();
            },
            cancellationToken);
    }
}
