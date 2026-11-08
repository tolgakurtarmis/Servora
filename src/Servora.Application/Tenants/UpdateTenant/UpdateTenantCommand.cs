using Servora.Application.Abstractions;

namespace Servora.Application.Tenants.UpdateTenant;

public sealed record UpdateTenantCommand(Guid TenantId, string Name, bool IsActive) : ICommand<Guid>;

public sealed record UpdateTenantRequest(string Name, bool IsActive);
