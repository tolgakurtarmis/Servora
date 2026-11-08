using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Catalog.UpdateMenuItem;

public sealed class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, Guid>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly IMenuItemRepository _menuItems;

    public UpdateMenuItemCommandHandler(ICurrentTenant currentTenant, IMenuItemRepository menuItems)
    {
        _currentTenant = currentTenant;
        _menuItems = menuItems;
    }

    public async Task<Guid> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsPlatformAdmin && _currentTenant.TenantId != request.TenantId)
        {
            throw new ForbiddenException("You can only update the menu of your own tenant.");
        }

        var menuItem = await _menuItems.GetByIdAsync(request.MenuItemId, cancellationToken);
        if (menuItem is null || menuItem.TenantId != request.TenantId || menuItem.BranchId != request.BranchId)
        {
            throw new NotFoundException("Menu item was not found.");
        }

        var name = request.Name.Trim();
        if (await _menuItems.ExistsByNameAsync(menuItem.BranchId, name, menuItem.Id, cancellationToken))
        {
            throw new BusinessException("A menu item with this name already exists in the branch.");
        }

        menuItem.Name = name;
        menuItem.Price = request.Price;
        menuItem.IsActive = request.IsActive;
        return menuItem.Id;
    }
}
