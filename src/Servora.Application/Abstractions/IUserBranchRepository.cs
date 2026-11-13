using Servora.Domain.Models;

namespace Servora.Application.Abstractions;

public interface IUserBranchRepository : IRepository<UserBranch>
{
    Task<UserBranch?> GetFirstByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid userId, Guid branchId, CancellationToken cancellationToken);
}
