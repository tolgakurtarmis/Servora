using Microsoft.EntityFrameworkCore;
using Servora.Application.Abstractions;
using Servora.Domain.Entities;

namespace Servora.Infrastructure.Persistence;

internal class EfRepository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    private readonly ServoraDbContext _dbContext;

    public EfRepository(ServoraDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Set<TEntity>().FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<TEntity?> GetByIdReadOnlyAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public void Remove(TEntity entity)
    {
        entity.IsDeleted = true;
        _dbContext.Set<TEntity>().Update(entity);
    }
}
