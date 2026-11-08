using AutoMapper;
using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Ordering.OpenTableSession;

public sealed class OpenTableSessionCommandHandler : IRequestHandler<OpenTableSessionCommand, Guid>
{
    private readonly IMapper _mapper;
    private readonly ICurrentTenant _currentTenant;
    private readonly IRepository<Table> _tables;
    private readonly ITableSessionRepository _sessions;

    public OpenTableSessionCommandHandler(
        IMapper mapper,
        ICurrentTenant currentTenant,
        IRepository<Table> tables,
        ITableSessionRepository sessions)
    {
        _mapper = mapper;
        _currentTenant = currentTenant;
        _tables = tables;
        _sessions = sessions;
    }

    public async Task<Guid> Handle(OpenTableSessionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsPlatformAdmin && _currentTenant.TenantId != request.TenantId)
        {
            throw new ForbiddenException("You can only open a table in your own tenant.");
        }

        var table = await _tables.GetByIdAsync(request.TableId, cancellationToken);
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
