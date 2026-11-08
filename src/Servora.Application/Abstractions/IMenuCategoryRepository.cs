using Servora.Domain.Models;

namespace Servora.Application.Abstractions;

public interface IMenuCategoryRepository : IRepository<MenuCategory>
{
    Task<bool> ExistsByNameAsync(
        Guid branchId,
        string name,
        Guid? excludeId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<MenuCategory>> ListByBranchIdAsync(Guid branchId, CancellationToken cancellationToken);

    Task<bool> HasMenuItemsAsync(Guid categoryId, CancellationToken cancellationToken);
}
