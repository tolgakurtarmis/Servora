using Microsoft.EntityFrameworkCore;
using Servora.Application.Abstractions;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence;

internal sealed class TableRepository : EfRepository<Table>, ITableRepository
{
    private readonly ServoraDbContext _dbContext;

    public TableRepository(ServoraDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsByNameAsync(Guid branchId, string name, CancellationToken cancellationToken)
    {
        return _dbContext.Tables.AnyAsync(
            table => table.BranchId == branchId && table.Name == name,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Table>> ListByBranchIdAsync(
        Guid branchId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Tables
            .AsNoTracking()
            .Where(table => table.BranchId == branchId)
            .OrderBy(table => table.Name)
            .ToListAsync(cancellationToken);
    }
}
