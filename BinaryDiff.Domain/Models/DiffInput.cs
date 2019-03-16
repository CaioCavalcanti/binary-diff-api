using BinaryDiff.Domain.Enum;
using System;

namespace BinaryDiff.Domain.Models
{
    /// <summary>
    /// Input model to be compared
    /// </summary>
    public class DiffInput
    {
        /// <summary>
        /// Creates an input for the direction informed
        /// </summary>
        /// <param name="direction">Direction to be used on diff</param>
        /// <param name="data">Base 64 data</param>
        public DiffInput(DiffDirection direction, string data)
        {
            Timestamp = DateTime.UtcNow;
            Data = data;
            Direction = direction;
        }

        /// <summary>
        /// Input direction to diff
        /// </summary>
        public DiffDirection Direction { get; set; }

        /// <summary>
        /// Date and time (UTC) that the input was received
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Base 64 data
        /// </summary>
        public string Data { get; set; }
    }
}
