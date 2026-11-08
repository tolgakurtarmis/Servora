using Microsoft.EntityFrameworkCore;
using Servora.Application.Abstractions;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence;

internal sealed class MenuCategoryRepository : EfRepository<MenuCategory>, IMenuCategoryRepository
{
    private readonly ServoraDbContext _dbContext;

    public MenuCategoryRepository(ServoraDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsByNameAsync(
        Guid branchId,
        string name,
        Guid? excludeId,
        CancellationToken cancellationToken)
    {
        return _dbContext.MenuCategories.AnyAsync(
            category => category.BranchId == branchId
                && category.Name == name
                && (excludeId == null || category.Id != excludeId),
            cancellationToken);
    }

    public async Task<IReadOnlyList<MenuCategory>> ListByBranchIdAsync(
        Guid branchId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.MenuCategories
            .AsNoTracking()
            .Where(category => category.BranchId == branchId && category.IsActive)
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> HasMenuItemsAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        return _dbContext.MenuItems.AnyAsync(item => item.CategoryId == categoryId, cancellationToken);
    }
}
