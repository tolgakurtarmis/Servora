using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Catalog.DeleteMenuCategory;

public sealed class DeleteMenuCategoryCommandHandler(
    IAppCache cache,
    IMenuCategoryRepository categories) : IRequestHandler<DeleteMenuCategoryCommand, Guid>
{
    public async Task<Guid> Handle(DeleteMenuCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categories.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null || category.TenantId != request.TenantId || category.BranchId != request.BranchId)
        {
            throw new NotFoundException("Menu category was not found.");
        }

        if (await categories.HasMenuItemsAsync(category.Id, cancellationToken))
        {
            throw new BusinessException("A menu category that still has menu items cannot be deleted.");
        }

        categories.Remove(category);
        cache.Remove(CacheKeys.Categories(category.BranchId));
        cache.Remove(CacheKeys.Menus(category.BranchId));

        return category.Id;
    }
}
