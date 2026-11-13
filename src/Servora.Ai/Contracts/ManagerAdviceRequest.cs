namespace Servora.Ai.Contracts;

public sealed record ManagerAdviceRequest(Guid BranchId, string? Focus = null);
