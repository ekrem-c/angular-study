using System;
using System.Runtime.Serialization;

namespace Platform.Common
{
    public class RemovedException : Exception
    {
        public RemovedException()
        {
        }

        public RemovedException(string message) : base(message)
        {
        }

        public RemovedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RemovedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}