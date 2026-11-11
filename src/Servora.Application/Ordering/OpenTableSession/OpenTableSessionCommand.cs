using Servora.Application.Abstractions;

namespace Servora.Application.Ordering.OpenTableSession;

public sealed record OpenTableSessionCommand(Guid BranchId, Guid TableId) : ICommand<Guid>, IBranchRequest;
