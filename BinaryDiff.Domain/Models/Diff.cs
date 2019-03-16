using System;

namespace BinaryDiff.Domain.Models
{
    /// <summary>
    /// Entity that holds the values to be compared
    /// </summary>
    public class Diff
    {
        /// <summary>
        /// Generates a new Id for the instance
        /// </summary>
        public Diff()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Diff unique ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Base 64 encoded string on left position
        /// </summary>
        public string Left { get; set; }

        /// <summary>
        /// Base 64 encoded string on right position
        /// </summary>
        public string Right { get; set; }
    }
}
