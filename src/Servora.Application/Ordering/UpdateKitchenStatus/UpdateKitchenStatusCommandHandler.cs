using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Enums;

namespace Servora.Application.Ordering.UpdateKitchenStatus;

public sealed class UpdateKitchenStatusCommandHandler(
    ICachedBranchReader branches,
    IOrderLineRepository lines,
    IBranchRealtime realtime) : IRequestHandler<UpdateKitchenStatusCommand, Guid>
{
    private readonly ICachedBranchReader _branches = branches;
    private readonly IOrderLineRepository _lines = lines;
    private readonly IBranchRealtime _realtime = realtime;

    public async Task<Guid> Handle(UpdateKitchenStatusCommand request, CancellationToken cancellationToken)
    {
        var branch = await _branches.GetAsync(request.BranchId, cancellationToken);
        if (branch is null)
        {
            throw new NotFoundException("Branch was not found.");
        }

        if (!branch.UsesKitchen)
        {
            throw new BusinessException("This branch does not use a kitchen workflow.");
        }

        var line = await _lines.GetByIdAsync(request.OrderLineId, cancellationToken);
        if (line is null || line.BranchId != request.BranchId)
        {
            throw new NotFoundException("Order line was not found.");
        }

        var isAllowed = (line.KitchenStatus, request.Status) switch
        {
            (KitchenStatus.Pending, KitchenStatus.Preparing) => true,
            (KitchenStatus.Preparing, KitchenStatus.Ready) => true,
            (KitchenStatus.Ready, KitchenStatus.Served) => true,
            _ => false
        };

        if (!isAllowed)
        {
            throw new BusinessException(
                $"Kitchen status cannot move from {line.KitchenStatus} to {request.Status}.");
        }

        line.KitchenStatus = request.Status;
        _realtime.KitchenQueueChanged(line.BranchId);
        return line.Id;
    }
}
