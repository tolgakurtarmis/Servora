using Servora.Application.Abstractions;
using Servora.Domain.Enums;

namespace Servora.Application.Tenants.CreateTenant;

public sealed record CreateTenantCommand(
    string Name,
    int MaxBranches,
    int MaxUsers,
    SubscriptionStatus? Status) : ICommand<Guid>;
