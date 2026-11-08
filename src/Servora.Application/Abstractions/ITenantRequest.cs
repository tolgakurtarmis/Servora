namespace Servora.Application.Abstractions;

/// <summary>
/// Marks a request that is scoped to a tenant.
/// Requests without this marker skip tenant access checks in the pipeline.
/// </summary>
public interface ITenantRequest
{
    Guid TenantId { get; }
}
