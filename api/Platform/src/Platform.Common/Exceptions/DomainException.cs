using System;

namespace Platform.Common
{
    /// <summary>
    ///     Implements the exceptions that are thrown when domain objects are initialized with invalid data.
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Platform.Common.Exceptions.DomainException" /> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public DomainException(string message)
        {
        }
    }
}