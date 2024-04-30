namespace AuctionHouse.Application.Common.Exceptions;

using System;

[Serializable]
public class UserLoginException : Exception
{
    public UserLoginException()
        : base("The provided credentials are incorrect!")
    {
    }
}