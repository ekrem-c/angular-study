using System;
using System.Runtime.Serialization;

namespace Platform.Common
{
    public class NotModifiedException : Exception
    {
        public NotModifiedException()
        {
        }

        public NotModifiedException(string message) : base(message)
        {
        }

        public NotModifiedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotModifiedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}