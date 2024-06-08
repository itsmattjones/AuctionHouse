using System.Collections.Generic;

namespace AuctionHouse.Domain.Common.Result;

public class Result
{
    protected Result(bool isSuccess, Error error, params string[] errorMessages)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorMessages = errorMessages?.Length > 0 ? errorMessages : [];
    }

    public Error Error { get; }
    public IEnumerable<string> ErrorMessages { get; } = [];

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public static Result Success() => new(true, Error.None);
    public static Result<T> Success<T>(T value) => new(value, true, Error.None);
    public static Result Failure(Error error, params string[] errorMessages) => new(false, error, errorMessages);
    public static Result<T> Failure<T>(Error error, params string[] errorMessages) => new(default!, false, error, errorMessages);
}