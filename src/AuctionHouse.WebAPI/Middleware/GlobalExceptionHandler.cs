using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuctionHouse.WebAPI.Middleware;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An exception has occured: {msg}", exception.Message);

        var problemDetails = exception is ValidationException validationException 
            ? CreateValidationProblemDetails(validationException) 
            : CreateProblemDetails();

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static ProblemDetails CreateValidationProblemDetails(ValidationException exception) => new()
    {
        Status = StatusCodes.Status400BadRequest,
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
        Title = "Bad Request",
        Detail = exception.Message
    };

    private static ProblemDetails CreateProblemDetails() => new()
    {
        Status = StatusCodes.Status500InternalServerError,
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
        Title = "Internal Server Error"
    };
}
