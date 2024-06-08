namespace AuctionHouse.Domain.Common.Result;

public class Result<T> : Result
{
    private readonly T _value;

    protected internal Result(T value, bool isSuccess, Error error, params string[] errorMessages)
        : base(isSuccess, error, errorMessages) => _value = value;

    public T Value => _value;

    public static implicit operator Result<T>(T value) => Success(value);
}