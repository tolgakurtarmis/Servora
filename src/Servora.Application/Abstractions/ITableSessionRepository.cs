using Servora.Domain.Models;

namespace Servora.Application.Abstractions;

public interface ITableSessionRepository : IRepository<TableSession>
{
    Task<bool> HasOpenSessionAsync(Guid tableId, CancellationToken cancellationToken);

    Task<IReadOnlyList<OpenTableSessionInfo>> ListOpenByBranchIdAsync(
        Guid branchId,
        CancellationToken cancellationToken);
}

public sealed record OpenTableSessionInfo(
    Guid TableId,
    string TableName,
    Guid SessionId,
    DateTime OpenedAt);
