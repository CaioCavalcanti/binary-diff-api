using BinaryDiff.Domain.Enum;
using BinaryDiff.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace BinaryDiff.Domain.Helpers
{
    /// <summary>
    /// Extesions for Diff model
    /// </summary>
    public static class DiffExtensions
    {
        /// <summary>
        /// Gets the last (or default) item for a given direction from a diff inputs
        /// </summary>
        /// <param name="diff">Diff to get inputs from</param>
        /// <param name="direction">Input direction (Left/Right) to get</param>
        /// <returns></returns>
        public static DiffInput GetLastInputOn(this Diff diff, DiffDirection direction) => diff.Inputs?.LastOrDefault(_ => _.Direction == direction);

        public static DiffInput AddInput(this Diff diff, DiffDirection direction, string data)
        {
            var input = new DiffInput(direction, data);

            if (diff.Inputs == null)
            {
                diff.Inputs = new List<DiffInput>();
            }

            diff.Inputs.Add(input);

            return input;
        }
    }
}
