using Servora.Application.Abstractions;

namespace Servora.Application.Catalog.ListMenuCategories;

public sealed record ListMenuCategoriesQuery(Guid TenantId, Guid BranchId)
    : IQuery<IReadOnlyList<MenuCategoryResponse>>, ITenantRequest;

public sealed record MenuCategoryResponse(Guid Id, string Name, int SortOrder);
