using BinaryDiff.Worker.Domain.Enums;
using BinaryDiff.Worker.Domain.Models;
using BinaryDiff.Worker.Domain.Utils;
using System.Collections.Generic;

namespace BinaryDiff.Worker.Domain.Logic
{
    public class DiffLogic : IDiffLogic
    {
        public DiffResult CompareData(string left, string right)
        {
            Dictionary<int, int> differences = null;
            ResultType result;

            if (left.IsLargerThan(right))
            {
                result = ResultType.LeftIsLarger;
            }
            else if (right.IsLargerThan(left))
            {
                result = ResultType.RightIsLarger;
            }
            else
            {
                result = left.EqualsToSameSizeString(right, out differences)
                    ? ResultType.Equal
                    : ResultType.Different;
            }

            return new DiffResult(result, differences);
        }

        public InputPosition GetOpposingPosition(InputPosition position)
        {
            return position == InputPosition.Left
                   ? InputPosition.Right
                   : InputPosition.Left;
        }
    }
}
