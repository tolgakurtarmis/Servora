using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Catalog.ListMenuCategories;

public sealed class ListMenuCategoriesQueryHandler(
    IAppCache cache,
    ICachedBranchReader branches,
    IMenuCategoryRepository categories)
    : IRequestHandler<ListMenuCategoriesQuery, IReadOnlyList<MenuCategoryResponse>>
{
    public async Task<IReadOnlyList<MenuCategoryResponse>> Handle(
        ListMenuCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var branch = await branches.GetAsync(request.BranchId, cancellationToken);
        if (branch is null || branch.TenantId != request.TenantId)
        {
            throw new NotFoundException("Branch was not found.");
        }

        return await cache.GetOrCreateAsync(
            CacheKeys.Categories(branch.Id),
            CacheDurations.Categories,
            async token =>
            {
                var items = await categories.ListByBranchIdAsync(branch.Id, token);
                return (IReadOnlyList<MenuCategoryResponse>)items
                    .Select(category => new MenuCategoryResponse(category.Id, category.Name, category.SortOrder))
                    .ToList();
            },
            cancellationToken);
    }
}
