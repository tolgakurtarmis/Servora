using Servora.Application.Abstractions;
using Servora.Domain.Enums;

namespace Servora.Application.Ordering.UpdateKitchenStatus;

public sealed record UpdateKitchenStatusCommand(
    Guid TenantId,
    Guid BranchId,
    Guid OrderLineId,
    KitchenStatus Status) : ICommand<Guid>;
