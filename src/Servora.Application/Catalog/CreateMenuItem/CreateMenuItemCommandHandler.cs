using AutoMapper;
using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Catalog.CreateMenuItem;

public sealed class CreateMenuItemCommandHandler(
    IMapper mapper,
    IAppCache cache,
    IRepository<Branch> branches,
    IMenuCategoryRepository categories,
    IMenuItemRepository menuItems) : IRequestHandler<CreateMenuItemCommand, Guid>
{
    public async Task<Guid> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var branch = await branches.GetByIdReadOnlyAsync(request.BranchId, cancellationToken);
        if (branch is null || branch.TenantId != request.TenantId)
        {
            throw new NotFoundException("Branch was not found.");
        }

        var category = await categories.GetByIdReadOnlyAsync(request.CategoryId, cancellationToken);
        if (category is null
            || category.TenantId != request.TenantId
            || category.BranchId != request.BranchId
            || !category.IsActive)
        {
            throw new NotFoundException("Menu category was not found.");
        }

        var name = request.Name.Trim();
        if (await menuItems.ExistsByNameAsync(branch.Id, name, null, cancellationToken))
        {
            throw new BusinessException("A menu item with this name already exists in the branch.");
        }

        var menuItem = mapper.Map<MenuItem>(request);
        menuItem.TenantId = branch.TenantId;
        await menuItems.AddAsync(menuItem, cancellationToken);
        cache.Remove(CacheKeys.Menus(branch.Id));

        return menuItem.Id;
    }
}
