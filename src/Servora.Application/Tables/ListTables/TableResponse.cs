namespace Servora.Application.Tables.ListTables;

public sealed record TableResponse(
    Guid Id,
    string Name,
    bool IsOccupied,
    Guid? SessionId,
    DateTime? OpenedAt);
