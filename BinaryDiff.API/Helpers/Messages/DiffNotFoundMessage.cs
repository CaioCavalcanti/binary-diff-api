using System;

namespace BinaryDiff.API.Helpers.Messages
{
    /// <summary>
    /// Default message to return when a diff is not found.
    /// </summary>
    public class DiffNotFoundMessage : BaseMessage
    {
        /// <summary>
        /// Generate message for diff key not found
        /// </summary>
        /// <param name="id">Diff ID that was not found</param>
        public DiffNotFoundMessage(Guid id) : base($"We couldn't find a diff with ID '{id.ToString()}'")
        {
        }
    }
}
