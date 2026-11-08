using Servora.Domain.Entities;

namespace Servora.Application.Abstractions;

public interface IRepository<TEntity>
    where TEntity : Entity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken);

    void Remove(TEntity entity);
}
