using System;

namespace BinaryDiff.API.Helpers.Messages
{
    /// <summary>
    /// Default message to return on exceptions, to prevent showing sensitive information
    /// to clients and also provide a reference id on logs to help troubleshoot.
    /// </summary>
    public class ExceptionMessage : BaseMessage
    {
        /// <summary>
        /// Generate a exception message for the error id provided
        /// </summary>
        /// <param name="errorId"></param>
        public ExceptionMessage(Guid errorId) : base("An error occurred on server side. If the error persists, contact the admin informing the error id provided.")
        {
            ErrorId = errorId;
        }

        /// <summary>
        /// Error id for reference on logs
        /// </summary>
        public Guid ErrorId { get; }
    }
}
