using AutoMapper;
using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Tables.CreateTable;

public sealed class CreateTableCommandHandler : IRequestHandler<CreateTableCommand, Guid>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Branch> _branches;
    private readonly ITableRepository _tables;

    public CreateTableCommandHandler(
        IMapper mapper,
        IRepository<Branch> branches,
        ITableRepository tables)
    {
        _mapper = mapper;
        _branches = branches;
        _tables = tables;
    }

    public async Task<Guid> Handle(CreateTableCommand request, CancellationToken cancellationToken)
    {
        var branch = await _branches.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null || branch.TenantId != request.TenantId)
        {
            throw new NotFoundException("Branch was not found.");
        }

        var name = request.Name.Trim();
        if (await _tables.ExistsByNameAsync(branch.Id, name, null, cancellationToken))
        {
            throw new BusinessException("A table with this name already exists in the branch.");
        }

        var table = _mapper.Map<Table>(request);
        table.TenantId = branch.TenantId;
        await _tables.AddAsync(table, cancellationToken);

        return table.Id;
    }
}
