namespace Servora.Application.Ordering.ListOpenTables;

public sealed record OpenTableResponse(
    Guid TableId,
    string TableName,
    Guid SessionId,
    DateTime OpenedAt);
