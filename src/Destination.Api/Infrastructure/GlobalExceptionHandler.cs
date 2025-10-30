namespace Destination.Api.Infrastructure;

using Application.Common.Exceptions;
using Domain.Features.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using SharedKernel;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Unhandled exception {ExceptionType} with message {ExceptionMessage}",
            exception.GetType().Name,
            exception.Message);

        var result = exception switch
        {
            ForbiddenException fe => Result.Failure(Error.Conflict("Error.Forbidden", fe.Message)),
            UnauthorizedException => Result.Failure(AuthErrors.AuthenticationFailed),
            _ => Result.Failure(Error.Problem("Error.ServerFailure", "Unhandled exception occurred"))
        };

        var finalResult = CustomResults.Problem(result);

        var actionContext = new ActionContext(
            httpContext,
            httpContext.GetRouteData(),
            new ActionDescriptor());

        await finalResult.ExecuteResultAsync(actionContext);

        return true;
    }
}
