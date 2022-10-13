using System;
using System.Runtime.Serialization;

namespace AuctionHouse.Application.Common.Exceptions
{
    [Serializable]
    public class UserLoginException : Exception
    {
        public UserLoginException()
            : base("The provided credentials are incorrect!")
        {
        }

        protected UserLoginException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
