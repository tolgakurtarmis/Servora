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

    public async Task<IReadOnlyList<User>> ListAsync(Guid? tenantId, CancellationToken cancellationToken)
    {
        var query = _dbContext.Users.AsNoTracking().AsQueryable();

        if (tenantId is not null)
        {
            query = query.Where(user => user.TenantId == tenantId);
        }

        return await query
            .OrderBy(user => user.Name)
            .ToListAsync(cancellationToken);
    }
}
