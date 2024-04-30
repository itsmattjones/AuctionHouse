namespace AuctionHouse.Application.Common.Exceptions;

using System;

[Serializable]
public class RefreshTokenException : Exception
{
    public RefreshTokenException()
    {
    }

    public RefreshTokenException(string message)
        : base(message)
    {
    }

    public RefreshTokenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}