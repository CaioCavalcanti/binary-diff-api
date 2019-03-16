using System.Collections.Generic;

namespace BinaryDiff.API.ViewModels
{
    /// <summary>
    /// View model for diff result
    /// </summary>
    public class DiffResultViewModel
    {
        /// <summary>
        /// User friendly result description
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Key/value pairs representing the differences with offset (key) and length (value).
        /// Offset and length set on left input when compared to right
        /// Optional: available only when inputs form both sides have the same size but differences on content
        /// </summary>
        public IDictionary<int, int> Differences { get; set; }
    }
}
