using System;

namespace BinaryDiff.Input.WebApi.Helpers.Messages
{
    /// <summary>
    /// Default message to return when a diff is not found.
    /// </summary>
    public class DiffNotFoundMessage : BaseMessage
    {
        /// <summary>
        /// Generate message for diff UUID not found
        /// </summary>
        /// <param name="id">Diff UUID</param>
        public DiffNotFoundMessage(Guid id) : base($"We couldn't find a diff with ID '{id.ToString()}'")
        {
        }
    }
}
