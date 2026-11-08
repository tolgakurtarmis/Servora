using Microsoft.EntityFrameworkCore;
using Servora.Application.Abstractions;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence;

internal sealed class UserBranchRepository : EfRepository<UserBranch>, IUserBranchRepository
{
    private readonly ServoraDbContext _dbContext;

    public UserBranchRepository(ServoraDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<UserBranch?> GetFirstByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbContext.Set<UserBranch>()
            .Where(membership => membership.UserId == userId)
            .OrderBy(membership => membership.CreateDate)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
