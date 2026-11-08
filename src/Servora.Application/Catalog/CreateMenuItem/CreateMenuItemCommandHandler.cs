using AutoMapper;
using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Catalog.CreateMenuItem;

public sealed class CreateMenuItemCommandHandler(
    IMapper mapper,
    IAppCache cache,
    IRepository<Branch> branches,
    IMenuItemRepository menuItems) : IRequestHandler<CreateMenuItemCommand, Guid>
{
    private readonly IMapper _mapper = mapper;
    private readonly IAppCache _cache = cache;
    private readonly IRepository<Branch> _branches = branches;
    private readonly IMenuItemRepository _menuItems = menuItems;

    public async Task<Guid> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var branch = await _branches.GetByIdReadOnlyAsync(request.BranchId, cancellationToken);
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
        _cache.Remove(CacheKeys.Menus(branch.Id));

        return menuItem.Id;
    }
}
