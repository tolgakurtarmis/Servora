using Servora.Application.Abstractions;

namespace Servora.Application.Catalog.DeleteMenuCategory;

public sealed record DeleteMenuCategoryCommand(
    Guid BranchId,
    Guid CategoryId) : ICommand<Guid>, IBranchRequest;
