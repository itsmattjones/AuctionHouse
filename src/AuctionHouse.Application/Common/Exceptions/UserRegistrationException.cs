using System;
using System.Runtime.Serialization;

namespace AuctionHouse.Application.Common.Exceptions
{
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

        protected UserRegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
