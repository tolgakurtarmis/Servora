namespace Servora.Application.Abstractions;

public interface ICachedBranchReader
{
    Task<CachedBranch?> GetAsync(Guid branchId, CancellationToken cancellationToken);
}
