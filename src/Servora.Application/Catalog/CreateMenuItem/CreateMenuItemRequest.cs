namespace Servora.Application.Catalog.CreateMenuItem;

public sealed record CreateMenuItemRequest(Guid CategoryId, string Name, decimal Price);
