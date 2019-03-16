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

            if (diff.Left.IsLargerThan(diff.Right))
            {
                return ResultType.LeftIsLarger;
            }
            else if (diff.Right.IsLargerThan(diff.Left))
            {
                return ResultType.RightIsLarger;
            }

            return diff.Left.EqualsToSameSizeString(diff.Right, out differences)
                ? ResultType.AreEqual
                : ResultType.DifferentContent;
        }
    }
}
