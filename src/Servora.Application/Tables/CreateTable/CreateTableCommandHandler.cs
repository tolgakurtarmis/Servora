using AutoMapper;
using MediatR;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Tables.CreateTable;

public sealed class CreateTableCommandHandler(
    IMapper mapper,
    IAppCache cache,
    IRepository<Branch> branches,
    ITableRepository tables) : IRequestHandler<CreateTableCommand, Guid>
{
    private readonly IMapper _mapper = mapper;
    private readonly IAppCache _cache = cache;
    private readonly IRepository<Branch> _branches = branches;
    private readonly ITableRepository _tables = tables;

    public async Task<Guid> Handle(CreateTableCommand request, CancellationToken cancellationToken)
    {
        var branch = await _branches.GetByIdReadOnlyAsync(request.BranchId, cancellationToken);
        if (branch is null)
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
        _cache.Remove(CacheKeys.Tables(branch.Id));

        return table.Id;
    }
}
