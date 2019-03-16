using BinaryDiff.Domain.Models;

namespace BinaryDiff.Domain.Logic
{
    /// <summary>
    /// Logic used to diff data
    /// </summary>
    public interface IDiffLogic
    {
        /// <summary>
        /// Diffs data on left and right position
        /// </summary>
        /// <param name="diff">Diff instance</param>
        /// <returns cref="BinaryDiff.Domain.Models.DiffResult">DiffResult with difference details</returns>
        DiffResult GetResult(Diff diff);
    }
}
