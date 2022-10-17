using System;
using System.Runtime.Serialization;

namespace AuctionHouse.Application.Common.Exceptions
{
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

        protected RefreshTokenException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
