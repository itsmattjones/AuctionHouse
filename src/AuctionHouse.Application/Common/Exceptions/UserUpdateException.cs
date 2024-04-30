namespace AuctionHouse.Application.Common.Exceptions;

using System;

[Serializable]
public class UserUpdateException : Exception
{
    public UserUpdateException()
    {
    }

    public UserUpdateException(string message)
        : base(message)
    {
    }

    public UserUpdateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
