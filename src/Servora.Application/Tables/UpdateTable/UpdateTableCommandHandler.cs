using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Tables.UpdateTable;

public sealed class UpdateTableCommandHandler : IRequestHandler<UpdateTableCommand, Guid>
{
    private readonly IAppCache _cache;
    private readonly ITableRepository _tables;

    public UpdateTableCommandHandler(IAppCache cache, ITableRepository tables)
    {
        _cache = cache;
        _tables = tables;
    }

    public async Task<Guid> Handle(UpdateTableCommand request, CancellationToken cancellationToken)
    {
        var table = await _tables.GetByIdAsync(request.TableId, cancellationToken);
        if (table is null || table.TenantId != request.TenantId || table.BranchId != request.BranchId)
        {
            throw new NotFoundException("Table was not found.");
        }

        var name = request.Name.Trim();
        if (await _tables.ExistsByNameAsync(table.BranchId, name, table.Id, cancellationToken))
        {
            throw new BusinessException("A table with this name already exists in the branch.");
        }

        table.Name = name;
        table.IsActive = request.IsActive;
        _cache.Remove(CacheKeys.Tables(table.BranchId));
        return table.Id;
    }
}
