namespace AuctionHouse.Application.Common.Exceptions;

using System;

[Serializable]
public class UserRegistrationException : Exception
{
    public UserRegistrationException()
    {
    }

    public UserRegistrationException(string message)
        : base(message)
    {
    }

    public UserRegistrationException(string message, string[] errors)
        : base(message)
    {
        Data["errors"] = errors;
    }

    public UserRegistrationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}