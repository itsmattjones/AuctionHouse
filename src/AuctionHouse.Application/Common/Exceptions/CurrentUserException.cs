namespace AuctionHouse.Application.Common.Exceptions;

using System;

[Serializable]
public class CurrentUserException : Exception
{
    public CurrentUserException()
    {
    }

    public CurrentUserException(string message)
        : base(message)
    {
    }

    public CurrentUserException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}