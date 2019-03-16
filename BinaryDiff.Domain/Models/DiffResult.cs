using BinaryDiff.Domain.Enum;
using System;
using System.Collections.Generic;

namespace BinaryDiff.Domain.Models
{
    /// <summary>
    /// Object that holds the diff result of a Diff object
    /// </summary>
    public class DiffResult
    {
        /// <summary>
        /// Diff unique ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Result from left/right diff
        /// </summary>
        public ResultType Result { get; set; }

        /// <summary>
        /// Differences from left and right data, each entry represents a difference
        /// where the key is the offset and value is the length.
        /// </summary>
        public IDictionary<int, int> Differences { get; set; }
    }
}
