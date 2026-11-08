using MediatR;
using Servora.Application.Abstractions;

namespace Servora.Application.Behaviors;

public sealed class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBranchRealtime _realtime;

    public UnitOfWorkBehavior(IUnitOfWork unitOfWork, IBranchRealtime realtime)
    {
        _unitOfWork = unitOfWork;
        _realtime = realtime;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        if (request is ICommandBase)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _realtime.FlushAsync(cancellationToken);
        }

        return response;
    }
}
