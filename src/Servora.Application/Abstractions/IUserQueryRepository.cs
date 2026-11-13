using Servora.Domain.Models;

namespace Servora.Application.Abstractions;

public interface IUserQueryRepository
{
    Task<IReadOnlyList<User>> ListAsync(
        Guid? tenantId,
        Guid? branchId,
        CancellationToken cancellationToken);
}
