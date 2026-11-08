using Servora.Application.Abstractions;

namespace Servora.Application.Tenants.ListTenants;

public sealed record ListTenantsQuery : IQuery<IReadOnlyList<TenantResponse>>;

public sealed record TenantResponse(Guid Id, string Name, bool IsActive);
