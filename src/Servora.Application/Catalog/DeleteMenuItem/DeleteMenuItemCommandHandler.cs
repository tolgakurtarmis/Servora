using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Catalog.DeleteMenuItem;

public sealed class DeleteMenuItemCommandHandler : IRequestHandler<DeleteMenuItemCommand, Guid>
{
    private readonly IMenuItemRepository _menuItems;

    public DeleteMenuItemCommandHandler(IMenuItemRepository menuItems)
    {
        _menuItems = menuItems;
    }

    public async Task<Guid> Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuItems.GetByIdAsync(request.MenuItemId, cancellationToken);
        if (menuItem is null || menuItem.TenantId != request.TenantId || menuItem.BranchId != request.BranchId)
        {
            throw new NotFoundException("Menu item was not found.");
        }

        _menuItems.Remove(menuItem);
        return menuItem.Id;
    }
}
