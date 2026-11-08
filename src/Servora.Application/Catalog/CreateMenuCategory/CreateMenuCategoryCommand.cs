using Servora.Application.Abstractions;

namespace Servora.Application.Catalog.CreateMenuCategory;

public sealed record CreateMenuCategoryCommand(
    Guid TenantId,
    Guid BranchId,
    string Name,
    int SortOrder) : ICommand<Guid>, ITenantRequest;

public sealed record CreateMenuCategoryRequest(string Name, int SortOrder);

public sealed record CreateMenuCategoryResponse(Guid Id);
