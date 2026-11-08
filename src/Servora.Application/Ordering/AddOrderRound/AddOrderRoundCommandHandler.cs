using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Application.Ordering.AddOrderRound;

public sealed class AddOrderRoundCommandHandler : IRequestHandler<AddOrderRoundCommand, Guid>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly ITableSessionRepository _sessions;
    private readonly IRepository<MenuItem> _menuItems;
    private readonly IRepository<OrderRound> _rounds;

    public AddOrderRoundCommandHandler(
        ICurrentTenant currentTenant,
        ITableSessionRepository sessions,
        IRepository<MenuItem> menuItems,
        IRepository<OrderRound> rounds)
    {
        _currentTenant = currentTenant;
        _sessions = sessions;
        _menuItems = menuItems;
        _rounds = rounds;
    }

    public async Task<Guid> Handle(AddOrderRoundCommand request, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsPlatformAdmin && _currentTenant.TenantId != request.TenantId)
        {
            throw new ForbiddenException("You can only add orders for your own tenant.");
        }

        var session = await _sessions.GetByIdAsync(request.TableSessionId, cancellationToken);
        if (session is null || session.TenantId != request.TenantId || session.BranchId != request.BranchId)
        {
            throw new NotFoundException("Table session was not found.");
        }

        if (session.Status != TableSessionStatus.Open)
        {
            throw new BusinessException("Orders can only be added to an open table session.");
        }

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
                KitchenStatus = KitchenStatus.Pending
            });
        }

        await _rounds.AddAsync(round, cancellationToken);
        return round.Id;
    }
}
