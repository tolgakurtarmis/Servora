using AutoMapper;
using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Catalog.CreateMenuItem;

public sealed class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, Guid>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Branch> _branches;
    private readonly IMenuItemRepository _menuItems;

    public CreateMenuItemCommandHandler(
        IMapper mapper,
        IRepository<Branch> branches,
        IMenuItemRepository menuItems)
    {
        _mapper = mapper;
        _branches = branches;
        _menuItems = menuItems;
    }

    public async Task<Guid> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var branch = await _branches.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null || branch.TenantId != request.TenantId)
        {
            throw new NotFoundException("Branch was not found.");
        }

        var name = request.Name.Trim();
        if (await _menuItems.ExistsByNameAsync(branch.Id, name, null, cancellationToken))
        {
            throw new BusinessException("A menu item with this name already exists in the branch.");
        }

        var menuItem = _mapper.Map<MenuItem>(request);
        menuItem.TenantId = branch.TenantId;
        await _menuItems.AddAsync(menuItem, cancellationToken);

        return menuItem.Id;
    }
}
