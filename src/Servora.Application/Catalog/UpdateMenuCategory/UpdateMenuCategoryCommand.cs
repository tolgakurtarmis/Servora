using Servora.Application.Abstractions;

namespace Servora.Application.Catalog.UpdateMenuCategory;

public sealed record UpdateMenuCategoryCommand(
    Guid BranchId,
    Guid CategoryId,
    string Name,
    int SortOrder,
    bool IsActive) : ICommand<Guid>, IBranchRequest;

public sealed record UpdateMenuCategoryRequest(string Name, int SortOrder, bool IsActive);
