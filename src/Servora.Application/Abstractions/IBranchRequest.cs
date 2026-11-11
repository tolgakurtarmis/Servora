namespace Servora.Application.Abstractions;

/// <summary>
/// Marks a request scoped to a branch. Tenant is resolved from the branch, not the client.
/// </summary>
public interface IBranchRequest
{
    Guid BranchId { get; }
}
