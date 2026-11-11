namespace Servora.Application.Abstractions;

/// <summary>
/// Marks a tenant-level request (no branch in the route).
/// Branch-scoped requests use <see cref="IBranchRequest"/> instead; tenant is taken from the branch.
/// </summary>
public interface ITenantRequest
{
    Guid TenantId { get; }
}
