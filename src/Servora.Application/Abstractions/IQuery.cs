using MediatR;

namespace Servora.Application.Abstractions;

public interface IQuery<TResponse> : IRequest<TResponse>;
