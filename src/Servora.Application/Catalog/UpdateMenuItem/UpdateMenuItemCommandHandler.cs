using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Catalog.UpdateMenuItem;

public sealed class UpdateMenuItemCommandHandler(
    IAppCache cache,
    IMenuCategoryRepository categories,
    IMenuItemRepository menuItems) : IRequestHandler<UpdateMenuItemCommand, Guid>
{
    public async Task<Guid> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var menuItem = await menuItems.GetByIdAsync(request.MenuItemId, cancellationToken);
        if (menuItem is null || menuItem.BranchId != request.BranchId)
        {
            throw new NotFoundException("Menu item was not found.");
        }

        var category = await categories.GetByIdReadOnlyAsync(request.CategoryId, cancellationToken);
        if (category is null
            || category.BranchId != request.BranchId
            || !category.IsActive)
        {
            throw new NotFoundException("Menu category was not found.");
        }

        var name = request.Name.Trim();
        if (await menuItems.ExistsByNameAsync(menuItem.BranchId, name, menuItem.Id, cancellationToken))
        {
            throw new BusinessException("A menu item with this name already exists in the branch.");
        }

        menuItem.CategoryId = category.Id;
        menuItem.Name = name;
        menuItem.Price = request.Price;
        menuItem.IsActive = request.IsActive;
        cache.Remove(CacheKeys.Menus(menuItem.BranchId));
        return menuItem.Id;
    }
}
