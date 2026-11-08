using Microsoft.EntityFrameworkCore;
using Servora.Application.Abstractions;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence;

internal sealed class MenuItemRepository : EfRepository<MenuItem>, IMenuItemRepository
{
    private readonly ServoraDbContext _dbContext;

    public MenuItemRepository(ServoraDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsByNameAsync(
        Guid branchId,
        string name,
        Guid? excludeId,
        CancellationToken cancellationToken)
    {
        return _dbContext.MenuItems.AnyAsync(
            item => item.BranchId == branchId
                && item.Name == name
                && (excludeId == null || item.Id != excludeId),
            cancellationToken);
    }

    public async Task<IReadOnlyList<MenuItem>> ListByBranchIdAsync(
        Guid branchId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.MenuItems
            .AsNoTracking()
            .Include(item => item.Category)
            .Where(item => item.BranchId == branchId && item.IsActive)
            .OrderBy(item => item.Category.SortOrder)
            .ThenBy(item => item.Category.Name)
            .ThenBy(item => item.Name)
            .ToListAsync(cancellationToken);
    }
}
