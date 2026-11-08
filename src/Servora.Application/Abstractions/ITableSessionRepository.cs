using Servora.Domain.Models;

namespace Servora.Application.Abstractions;

public interface ITableSessionRepository : IRepository<TableSession>
{
    Task<bool> HasOpenSessionAsync(Guid tableId, CancellationToken cancellationToken);
}
