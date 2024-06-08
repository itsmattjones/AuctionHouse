using System;
using System.Collections.Generic;
using System.Net;
using AuctionHouse.Domain.Common.Result;
using Microsoft.AspNetCore.Http;

namespace AuctionHouse.WebAPI.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException();

        return Results.Problem(
            statusCode: result.GetErrorStatusCode(),
            title: result.GetErrorTitle(),
            type: result.GetErrorType(),
            extensions: new Dictionary<string, object?>
            {
                { "errors", new [] { result.ErrorMessages }}
            }
        );
    }

    private static int GetErrorStatusCode(this Result result) => result.Error switch
    {
        Error.Invalid => StatusCodes.Status400BadRequest,
        Error.NotFound => StatusCodes.Status400BadRequest,
        Error.Conflict => StatusCodes.Status400BadRequest,
        Error.Critial => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string GetErrorTitle(this Result result) => result.Error switch
    {
        Error.Invalid => HttpStatusCode.BadRequest.ToString(),
        Error.NotFound => HttpStatusCode.BadRequest.ToString(),
        Error.Conflict => HttpStatusCode.BadRequest.ToString(),
        Error.Critial => HttpStatusCode.InternalServerError.ToString(),
        _ => HttpStatusCode.InternalServerError.ToString()
    };

    private static string GetErrorType(this Result result) => result.Error switch
    {
        Error.Invalid => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
        Error.NotFound => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
        Error.Conflict => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
        Error.Critial => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
        _ => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
    };
}