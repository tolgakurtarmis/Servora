using Servora.Application.Abstractions;

namespace Servora.Application.Ordering.CancelTableSession;

public sealed record CancelTableSessionCommand(
    Guid BranchId,
    Guid TableSessionId) : ICommand<Guid>, IBranchRequest;
