using System;
using System.Runtime.Serialization;

namespace Platform.Common
{
    public class InternalServerException : ExceptionBase
    {
        public InternalServerException()
        {
        }

        public InternalServerException(ModelError error) : base(error)
        {
        }

        public InternalServerException(ModelError error, Exception innerException) : base(error, innerException)
        {
        }

        public InternalServerException(string message) : base(message)
        {
        }

        public InternalServerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InternalServerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}