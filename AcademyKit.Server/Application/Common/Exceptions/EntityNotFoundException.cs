namespace AcademyKit.Application.Common.Exceptions
{
    public class EntityNotFoundException : PersistenceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// </summary>
        public EntityNotFoundException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class
        /// with a specified error message.
        /// </summary>
        ///
        /// <param name="message">The message that describes the error.</param>
        public EntityNotFoundException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class
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
        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
