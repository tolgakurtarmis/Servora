using Servora.Application.Abstractions;

namespace Servora.Application.Catalog.DeleteMenuItem;

public sealed record DeleteMenuItemCommand(Guid BranchId, Guid MenuItemId) : ICommand<Guid>, IBranchRequest;
