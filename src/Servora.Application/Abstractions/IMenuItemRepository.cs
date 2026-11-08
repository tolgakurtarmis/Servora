using Servora.Domain.Models;

namespace Servora.Application.Abstractions;

public interface IMenuItemRepository : IRepository<MenuItem>
{
    Task<bool> ExistsByNameAsync(Guid branchId, string name, CancellationToken cancellationToken);
}
