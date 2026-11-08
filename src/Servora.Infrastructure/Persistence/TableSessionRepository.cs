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
}
