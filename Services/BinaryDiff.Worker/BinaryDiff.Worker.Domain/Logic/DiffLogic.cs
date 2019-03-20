using BinaryDiff.Worker.Domain.Enums;
using BinaryDiff.Worker.Domain.Models;
using BinaryDiff.Worker.Domain.Utils;
using System.Collections.Generic;

namespace BinaryDiff.Worker.Domain.Logic
{
    public class DiffLogic : IDiffLogic
    {
        public DiffResult CompareData(InputData input, InputData opposingInput)
        {
            var left = GetInputOnPosition(InputPosition.Left, input, opposingInput);
            var right = GetInputOnPosition(InputPosition.Right, input, opposingInput);

            return CompareData(left?.Data, right?.Data);
        }

        public InputPosition GetOpposingPosition(InputPosition position)
        {
            return position == InputPosition.Left
                   ? InputPosition.Right
                   : InputPosition.Left;
        }

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

        private InputData GetInputOnPosition(InputPosition position, InputData input, InputData opposingInput)
        {
            return input.Position == position ? input : opposingInput;
        }
    }
}
