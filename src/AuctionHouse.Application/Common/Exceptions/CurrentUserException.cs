using System;
using System.Runtime.Serialization;

namespace AuctionHouse.Application.Common.Exceptions
{
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

        protected CurrentUserException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
