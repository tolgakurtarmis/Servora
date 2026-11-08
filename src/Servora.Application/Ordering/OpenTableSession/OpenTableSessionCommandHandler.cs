using AutoMapper;
using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Ordering.OpenTableSession;

public sealed class OpenTableSessionCommandHandler : IRequestHandler<OpenTableSessionCommand, Guid>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Table> _tables;
    private readonly ITableSessionRepository _sessions;

    public OpenTableSessionCommandHandler(
        IMapper mapper,
        IRepository<Table> tables,
        ITableSessionRepository sessions)
    {
        _mapper = mapper;
        _tables = tables;
        _sessions = sessions;
    }

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

        return session.Id;
    }
}
