using Servora.Application.Abstractions;
using Servora.Domain.Enums;

namespace Servora.Application.Ordering.UpdateKitchenStatus;

public sealed record UpdateKitchenStatusCommand(
    Guid BranchId,
    Guid OrderLineId,
    KitchenStatus Status) : ICommand<Guid>, IBranchRequest;
