using System;
using System.Runtime.Serialization;

namespace Platform.Common
{
    public class ExceptionBase : Exception
    {
        public ExceptionBase()
        {
        }

        public ExceptionBase(ModelError error) : this(error, null)
        {
        }

        public ExceptionBase(ModelError error, Exception innerException)
            : base($"{error.Code}-{error.Type}-{error.Message}", innerException)
        {
            ErrorMessage = new ModelErrorMessage(error);
        }

        public ExceptionBase(string message) : base(message)
        {
        }

        public ExceptionBase(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExceptionBase(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ModelErrorMessage ErrorMessage { get; set; }
    }
}