using Servora.Application.Abstractions;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Caching;

internal sealed class CachedBranchReader : ICachedBranchReader
{
    private readonly IAppCache _cache;
    private readonly IRepository<Branch> _branches;

    public CachedBranchReader(IAppCache cache, IRepository<Branch> branches)
    {
        _cache = cache;
        _branches = branches;
    }

    public async Task<CachedBranch?> GetAsync(Guid branchId, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(CacheKeys.Branch(branchId), out CachedBranch? cached) && cached is not null)
        {
            return cached;
        }

        var branch = await _branches.GetByIdReadOnlyAsync(branchId, cancellationToken);
        if (branch is null)
        {
            return null;
        }

        cached = new CachedBranch(branch.Id, branch.TenantId, branch.UsesKitchen, branch.IsActive);
        _cache.Set(CacheKeys.Branch(branch.Id), cached, CacheDurations.Branch);
        return cached;
    }
}
