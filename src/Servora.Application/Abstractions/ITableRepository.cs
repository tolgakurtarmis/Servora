using Servora.Domain.Models;

namespace Servora.Application.Abstractions;

public interface ITableRepository : IRepository<Table>
{
    Task<bool> ExistsByNameAsync(Guid branchId, string name, CancellationToken cancellationToken);
}
