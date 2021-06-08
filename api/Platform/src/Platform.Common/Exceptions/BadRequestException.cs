using System;
using System.Runtime.Serialization;

namespace Platform.Common
{
    public class BadRequestException : ExceptionBase
    {
        public BadRequestException()
        {
        }

        public BadRequestException(ModelError error) : base(error)
        {
        }

        public BadRequestException(ModelError error, Exception innerException) : base(error, innerException)
        {
        }

        public BadRequestException(string message) : base(message)
        {
        }

        public BadRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BadRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}