using AutoMapper;
using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Ordering.OpenTableSession;

public sealed class OpenTableSessionCommandHandler(
    IMapper mapper,
    IRepository<Table> tables,
    ITableSessionRepository sessions,
    IBranchRealtime realtime) : IRequestHandler<OpenTableSessionCommand, Guid>
{
    private readonly IMapper _mapper = mapper;
    private readonly IRepository<Table> _tables = tables;
    private readonly ITableSessionRepository _sessions = sessions;
    private readonly IBranchRealtime _realtime = realtime;

    public async Task<Guid> Handle(OpenTableSessionCommand request, CancellationToken cancellationToken)
    {
        var table = await _tables.GetByIdReadOnlyAsync(request.TableId, cancellationToken);
        if (table is null || table.TenantId != request.TenantId || table.BranchId != request.BranchId)
        {
            throw new NotFoundException("Table was not found.");
        }

        if (await _sessions.HasOpenSessionAsync(table.Id, cancellationToken))
        {
            throw new BusinessException("This table already has an open session.");
        }

        var session = _mapper.Map<TableSession>(request);
        await _sessions.AddAsync(session, cancellationToken);
        _realtime.TablesChanged(table.BranchId);

        return session.Id;
    }
}
