using Servora.Application.Abstractions;

namespace Servora.Application.Catalog.CreateMenuCategory;

public sealed record CreateMenuCategoryCommand(
    Guid BranchId,
    string Name,
    int SortOrder) : ICommand<Guid>, IBranchRequest;

public sealed record CreateMenuCategoryRequest(string Name, int SortOrder);

public sealed record CreateMenuCategoryResponse(Guid Id);
