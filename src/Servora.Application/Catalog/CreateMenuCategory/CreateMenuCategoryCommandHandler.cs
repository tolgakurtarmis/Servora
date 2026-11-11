using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Catalog.CreateMenuCategory;

public sealed class CreateMenuCategoryCommandHandler(
    IAppCache cache,
    IRepository<Branch> branches,
    IMenuCategoryRepository categories) : IRequestHandler<CreateMenuCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateMenuCategoryCommand request, CancellationToken cancellationToken)
    {
        var branch = await branches.GetByIdReadOnlyAsync(request.BranchId, cancellationToken);
        if (branch is null)
        {
            throw new NotFoundException("Branch was not found.");
        }

        var name = request.Name.Trim();
        if (await categories.ExistsByNameAsync(branch.Id, name, null, cancellationToken))
        {
            throw new BusinessException("A menu category with this name already exists in the branch.");
        }

        var category = new MenuCategory
        {
            TenantId = branch.TenantId,
            BranchId = branch.Id,
            Name = name,
            SortOrder = request.SortOrder
        };

        await categories.AddAsync(category, cancellationToken);
        cache.Remove(CacheKeys.Categories(branch.Id));
        cache.Remove(CacheKeys.Menus(branch.Id));

        return category.Id;
    }
}
