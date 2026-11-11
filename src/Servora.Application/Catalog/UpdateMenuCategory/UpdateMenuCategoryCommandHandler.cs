using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Catalog.UpdateMenuCategory;

public sealed class UpdateMenuCategoryCommandHandler(
    IAppCache cache,
    IMenuCategoryRepository categories) : IRequestHandler<UpdateMenuCategoryCommand, Guid>
{
    public async Task<Guid> Handle(UpdateMenuCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categories.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null || category.BranchId != request.BranchId)
        {
            throw new NotFoundException("Menu category was not found.");
        }

        var name = request.Name.Trim();
        if (await categories.ExistsByNameAsync(category.BranchId, name, category.Id, cancellationToken))
        {
            throw new BusinessException("A menu category with this name already exists in the branch.");
        }

        category.Name = name;
        category.SortOrder = request.SortOrder;
        category.IsActive = request.IsActive;
        cache.Remove(CacheKeys.Categories(category.BranchId));
        cache.Remove(CacheKeys.Menus(category.BranchId));

        return category.Id;
    }
}
