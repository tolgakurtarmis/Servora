using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Catalog.DeleteMenuItem;

public sealed class DeleteMenuItemCommandHandler(IAppCache cache, IMenuItemRepository menuItems) : IRequestHandler<DeleteMenuItemCommand, Guid>
{
    private readonly IAppCache _cache = cache;
    private readonly IMenuItemRepository _menuItems = menuItems;

    public async Task<Guid> Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuItems.GetByIdAsync(request.MenuItemId, cancellationToken);
        if (menuItem is null || menuItem.BranchId != request.BranchId)
        {
            throw new NotFoundException("Menu item was not found.");
        }

        _menuItems.Remove(menuItem);
        _cache.Remove(CacheKeys.Menus(menuItem.BranchId));
        return menuItem.Id;
    }
}
