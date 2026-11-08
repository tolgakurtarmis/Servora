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

    public Task<bool> ExistsByNameAsync(Guid branchId, string name, CancellationToken cancellationToken)
    {
        return _dbContext.MenuItems.AnyAsync(
            item => item.BranchId == branchId && item.Name == name,
            cancellationToken);
    }
}
