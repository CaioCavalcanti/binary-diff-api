using BinaryDiff.Domain.Enum;
using BinaryDiff.Domain.Helpers;
using BinaryDiff.Domain.Models;
using System.Collections.Generic;

namespace BinaryDiff.Domain.Logic.Implementation
{
    public class DiffLogic : IDiffLogic
    {
        public DiffResult GetResultFor(Diff diff)
        {
            var result = GetResultDetails(diff, out var differences);

            return new DiffResult
            {
                Id = diff.Id,
                Result = result,
                Differences = differences
            };
        }

        private ResultType GetResultDetails(Diff diff, out IDictionary<int, int> differences)
        {
            differences = null;

            if (diff.Left.Equals(diff.Right))
            {
                return ResultType.AreEqual;
            }
            else if (diff.Left.Length != diff.Right.Length)
            {
                return diff.Left.Length > diff.Right.Length
                    ? ResultType.LeftIsLarger
                    : ResultType.RightIsLarger;
            }

            differences = diff.Left.CompareToSameSizeString(diff.Right);

            return ResultType.DifferentContent;
        }
    }
}
