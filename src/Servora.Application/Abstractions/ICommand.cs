using MediatR;

namespace Servora.Application.Abstractions;

public interface ICommandBase;

public interface ICommand<TResponse> : IRequest<TResponse>, ICommandBase;
