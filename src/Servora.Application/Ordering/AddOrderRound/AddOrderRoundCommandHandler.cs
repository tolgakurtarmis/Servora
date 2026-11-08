using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Application.Ordering.AddOrderRound;

public sealed class AddOrderRoundCommandHandler(
    ITableSessionRepository sessions,
    ICachedBranchReader branches,
    IRepository<MenuItem> menuItems,
    IRepository<OrderRound> rounds,
    IBranchRealtime realtime) : IRequestHandler<AddOrderRoundCommand, Guid>
{
    private readonly ITableSessionRepository _sessions = sessions;
    private readonly ICachedBranchReader _branches = branches;
    private readonly IRepository<MenuItem> _menuItems = menuItems;
    private readonly IRepository<OrderRound> _rounds = rounds;
    private readonly IBranchRealtime _realtime = realtime;

    public async Task<Guid> Handle(AddOrderRoundCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessions.GetByIdAsync(request.TableSessionId, cancellationToken);
        if (session is null || session.TenantId != request.TenantId || session.BranchId != request.BranchId)
        {
            throw new NotFoundException("Table session was not found.");
        }

        if (session.Status != TableSessionStatus.Open)
        {
            throw new BusinessException("Orders can only be added to an open table session.");
        }

        var branch = await _branches.GetAsync(session.BranchId, cancellationToken) ?? throw new NotFoundException("Branch was not found.");

        var kitchenStatus = branch.UsesKitchen ? KitchenStatus.Pending : KitchenStatus.Ready;

        var round = new OrderRound
        {
            TenantId = session.TenantId,
            BranchId = session.BranchId,
            TableSessionId = session.Id
        };

        foreach (var line in request.Lines.GroupBy(item => (
            item.MenuItemId,
            Note: string.IsNullOrWhiteSpace(item.Note) ? string.Empty : item.Note.Trim())))
        {
            var menuItem = await _menuItems.GetByIdReadOnlyAsync(line.Key.MenuItemId, cancellationToken);
            if (menuItem is null || menuItem.TenantId != session.TenantId || menuItem.BranchId != session.BranchId)
            {
                throw new NotFoundException("Menu item was not found.");
            }

            if (!menuItem.IsActive)
            {
                throw new BusinessException($"'{menuItem.Name}' is not available.");
            }

            round.Lines.Add(new OrderLine
            {
                TenantId = session.TenantId,
                BranchId = session.BranchId,
                MenuItemId = menuItem.Id,
                Name = menuItem.Name,
                Note = line.Key.Note,
                UnitPrice = menuItem.Price,
                Quantity = line.Sum(item => item.Quantity),
                KitchenStatus = kitchenStatus
            });
        }

        await _rounds.AddAsync(round, cancellationToken);
        _realtime.KitchenQueueChanged(session.BranchId);
        return round.Id;
    }
}
