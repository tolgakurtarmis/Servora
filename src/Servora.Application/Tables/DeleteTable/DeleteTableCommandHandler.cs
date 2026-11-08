using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Application.Tables.DeleteTable;

public sealed class DeleteTableCommandHandler : IRequestHandler<DeleteTableCommand, Guid>
{
    private readonly ITableRepository _tables;
    private readonly ITableSessionRepository _sessions;

    public DeleteTableCommandHandler(
        ITableRepository tables,
        ITableSessionRepository sessions)
    {
        _tables = tables;
        _sessions = sessions;
    }

    public async Task<Guid> Handle(DeleteTableCommand request, CancellationToken cancellationToken)
    {
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
