namespace Servora.Application.Abstractions.Exceptions;

public sealed class BusinessException : AppException
{
    public BusinessException(string message) : base(message, 409)
    {
    }
}
