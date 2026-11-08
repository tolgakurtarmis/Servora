using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Application.Ordering.AddOrderRound;

public sealed class AddOrderRoundCommandHandler : IRequestHandler<AddOrderRoundCommand, Guid>
{
    private readonly ITableSessionRepository _sessions;
    private readonly IRepository<Branch> _branches;
    private readonly IRepository<MenuItem> _menuItems;
    private readonly IRepository<OrderRound> _rounds;

    public AddOrderRoundCommandHandler(
        ITableSessionRepository sessions,
        IRepository<Branch> branches,
        IRepository<MenuItem> menuItems,
        IRepository<OrderRound> rounds)
    {
        _sessions = sessions;
        _branches = branches;
        _menuItems = menuItems;
        _rounds = rounds;
    }

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

        var branch = await _branches.GetByIdAsync(session.BranchId, cancellationToken);
        if (branch is null)
        {
            throw new NotFoundException("Branch was not found.");
        }

        var kitchenStatus = branch.UsesKitchen ? KitchenStatus.Pending : KitchenStatus.Ready;

        var round = new OrderRound
        {
            TenantId = session.TenantId,
            BranchId = session.BranchId,
            TableSessionId = session.Id
        };

        foreach (var line in request.Lines.GroupBy(item => item.MenuItemId))
        {
            var menuItem = await _menuItems.GetByIdAsync(line.Key, cancellationToken);
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
                UnitPrice = menuItem.Price,
                Quantity = line.Sum(item => item.Quantity),
                KitchenStatus = kitchenStatus
            });
        }

        await _rounds.AddAsync(round, cancellationToken);
        return round.Id;
    }
}
