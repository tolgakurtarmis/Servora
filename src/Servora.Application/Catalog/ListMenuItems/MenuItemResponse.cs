namespace Servora.Application.Catalog.ListMenuItems;

public sealed record MenuItemResponse(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    int CategorySortOrder,
    string Name,
    decimal Price);
