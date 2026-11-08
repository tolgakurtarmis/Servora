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
        return await (
            from session in _dbContext.TableSessions
            join table in _dbContext.Tables on session.TableId equals table.Id
            where session.BranchId == branchId && session.Status == TableSessionStatus.Open
            orderby table.Name
            select new OpenTableSessionInfo(table.Id, table.Name, session.Id, session.CreateDate)
        ).ToListAsync(cancellationToken);
    }
}
