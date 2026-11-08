using Microsoft.EntityFrameworkCore;
using Servora.Application.Abstractions;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence;

internal sealed class TableSessionRepository : EfRepository<TableSession>, ITableSessionRepository
{
    private readonly ServoraDbContext _dbContext;

    public TableSessionRepository(ServoraDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> HasOpenSessionAsync(Guid tableId, CancellationToken cancellationToken)
    {
        return _dbContext.TableSessions.AnyAsync(
            session => session.TableId == tableId && session.Status == TableSessionStatus.Open,
            cancellationToken);
    }

    public async Task<IReadOnlyList<OpenTableSessionInfo>> ListOpenByBranchIdAsync(
        Guid branchId,
        CancellationToken cancellationToken)
    {
        var sessions = await _dbContext.TableSessions
            .AsNoTracking()
            .Include(session => session.Table)
            .Where(session => session.BranchId == branchId && session.Status == TableSessionStatus.Open)
            .OrderBy(session => session.Table.Name)
            .ToListAsync(cancellationToken);

        return sessions
            .Select(session => new OpenTableSessionInfo(
                session.Table.Id,
                session.Table.Name,
                session.Id,
                session.CreateDate))
            .ToList();
    }
}
