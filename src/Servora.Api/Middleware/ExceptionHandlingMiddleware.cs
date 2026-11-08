using FluentValidation;
using Servora.Application.Abstractions.Exceptions;

namespace Servora.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException exception)
        {
            var errors = exception.Errors
                .GroupBy(failure => string.IsNullOrWhiteSpace(failure.PropertyName) ? "request" : failure.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(failure => failure.ErrorMessage).ToArray());

            await Results.ValidationProblem(errors).ExecuteAsync(context);
        }
        catch (AppException exception)
        {
            await Results.Problem(
                title: exception.Message,
                statusCode: exception.StatusCode)
                .ExecuteAsync(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");

            await Results.Problem(
                title: "Beklenmeyen bir hata oluştu.",
                statusCode: StatusCodes.Status500InternalServerError,
                detail: _environment.IsDevelopment() ? exception.Message : null)
                .ExecuteAsync(context);
        }
    }
}
