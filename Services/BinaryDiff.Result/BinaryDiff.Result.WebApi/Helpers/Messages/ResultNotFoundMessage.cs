using System;

namespace BinaryDiff.Result.WebApi.Helpers.Messages
{
    /// <summary>
    /// Default message to return when a diff is not found.
    /// </summary>
    public class ResultNotFoundMessage : BaseMessage
    {
        /// <summary>
        /// Generate message for diff UUID not found
        /// </summary>
        /// <param name="id">Diff UUID</param>
        public ResultNotFoundMessage(Guid id) : base($"We couldn't find a result for ID '{id.ToString()}'")
        {
        }
    }
}
