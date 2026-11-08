using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Tables.DeleteTable;

public sealed class DeleteTableCommandHandler : IRequestHandler<DeleteTableCommand, Guid>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly ITableRepository _tables;
    private readonly ITableSessionRepository _sessions;

    public DeleteTableCommandHandler(
        ICurrentTenant currentTenant,
        ITableRepository tables,
        ITableSessionRepository sessions)
    {
        _currentTenant = currentTenant;
        _tables = tables;
        _sessions = sessions;
    }

    public async Task<Guid> Handle(DeleteTableCommand request, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsPlatformAdmin && _currentTenant.TenantId != request.TenantId)
        {
            throw new ForbiddenException("You can only delete tables for your own tenant.");
        }

        var table = await _tables.GetByIdAsync(request.TableId, cancellationToken);
        if (table is null || table.TenantId != request.TenantId || table.BranchId != request.BranchId)
        {
            throw new NotFoundException("Table was not found.");
        }

        if (await _sessions.HasOpenSessionAsync(table.Id, cancellationToken))
        {
            throw new BusinessException("A table with an open session cannot be deleted.");
        }

        _tables.Remove(table);
        return table.Id;
    }
}
