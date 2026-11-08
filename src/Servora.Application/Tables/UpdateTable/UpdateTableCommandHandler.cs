using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Tables.UpdateTable;

public sealed class UpdateTableCommandHandler : IRequestHandler<UpdateTableCommand, Guid>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly ITableRepository _tables;

    public UpdateTableCommandHandler(ICurrentTenant currentTenant, ITableRepository tables)
    {
        _currentTenant = currentTenant;
        _tables = tables;
    }

    public async Task<Guid> Handle(UpdateTableCommand request, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsPlatformAdmin && _currentTenant.TenantId != request.TenantId)
        {
            throw new ForbiddenException("You can only update tables for your own tenant.");
        }

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
        return table.Id;
    }
}
