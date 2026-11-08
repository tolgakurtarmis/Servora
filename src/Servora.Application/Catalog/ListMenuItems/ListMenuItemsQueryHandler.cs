using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Catalog.ListMenuItems;

public sealed class ListMenuItemsQueryHandler
    : IRequestHandler<ListMenuItemsQuery, IReadOnlyList<MenuItemResponse>>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly IRepository<Branch> _branches;
    private readonly IMenuItemRepository _menuItems;

    public ListMenuItemsQueryHandler(
        ICurrentTenant currentTenant,
        IRepository<Branch> branches,
        IMenuItemRepository menuItems)
    {
        _currentTenant = currentTenant;
        _branches = branches;
        _menuItems = menuItems;
    }

    public async Task<IReadOnlyList<MenuItemResponse>> Handle(
        ListMenuItemsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsPlatformAdmin && _currentTenant.TenantId != request.TenantId)
        {
            throw new ForbiddenException("You can only view the menu of your own tenant.");
        }

        var branch = await _branches.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null || branch.TenantId != request.TenantId)
        {
            throw new NotFoundException("Branch was not found.");
        }

        var items = await _menuItems.ListByBranchIdAsync(branch.Id, cancellationToken);
        return items
            .Select(item => new MenuItemResponse(item.Id, item.Name, item.Price))
            .ToList();
    }
}
