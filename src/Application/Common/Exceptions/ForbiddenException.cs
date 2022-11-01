namespace Lingtren.Application.Common.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    public class ForbiddenException : PersistenceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        public ForbiddenException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class
        /// with a specified error message.
        /// </summary>
        ///
        /// <param name="message">The message that describes the error.</param>
        public ForbiddenException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        ///
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference if no inner exception is
        /// specified.
        /// </param>
        public ForbiddenException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class with serialized data.
        /// </summary>
        ///
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        protected ForbiddenException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
