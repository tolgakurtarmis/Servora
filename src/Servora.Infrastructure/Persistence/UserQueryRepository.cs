using Microsoft.EntityFrameworkCore;
using Servora.Application.Abstractions;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence;

internal sealed class UserQueryRepository : IUserQueryRepository
{
    private readonly ServoraDbContext _dbContext;

    public UserQueryRepository(ServoraDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<User>> ListAsync(
        Guid? tenantId,
        Guid? branchId,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Users.AsNoTracking().AsQueryable();

        if (tenantId is not null)
        {
            query = query.Where(user => user.TenantId == tenantId);
        }

        if (branchId is not null)
        {
            query = query.Where(user =>
                _dbContext.Set<UserBranch>().Any(membership =>
                    membership.UserId == user.Id && membership.BranchId == branchId));
        }

        return await query
            .OrderBy(user => user.Name)
            .ToListAsync(cancellationToken);
    }
}
