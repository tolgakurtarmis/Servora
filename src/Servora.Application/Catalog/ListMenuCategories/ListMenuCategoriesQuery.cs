using Servora.Application.Abstractions;

namespace Servora.Application.Catalog.ListMenuCategories;

public sealed record ListMenuCategoriesQuery(Guid BranchId)
    : IQuery<IReadOnlyList<MenuCategoryResponse>>, IBranchRequest;

public sealed record MenuCategoryResponse(Guid Id, string Name, int SortOrder);
