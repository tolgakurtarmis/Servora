using Servora.Application.Abstractions;

namespace Servora.Application.Catalog.CreateMenuItem;

public sealed record CreateMenuItemCommand(
    Guid BranchId,
    Guid CategoryId,
    string Name,
    decimal Price) : ICommand<Guid>, IBranchRequest;
