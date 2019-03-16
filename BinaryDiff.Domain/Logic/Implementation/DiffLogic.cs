using BinaryDiff.Domain.Enum;
using BinaryDiff.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace BinaryDiff.Domain.Logic.Implementation
{
    public class DiffLogic : IDiffLogic
    {
        public DiffType GetResultFor(Diff diff, out Dictionary<int, int> differences)
        {
            differences = null;

            var left = GetLastInput(diff, DiffDirection.Left);
            var right = GetLastInput(diff, DiffDirection.Right);

            if (left.Data.Equals(right.Data))
            {
                return DiffType.Equal;
            }
            else if (left.Data.Length != right.Data.Length)
            {
                return DiffType.DifferentSize;
            }

            differences = GetDiffDetails(left.Data, right.Data);

            return DiffType.DifferentContent;
        }

        private Dictionary<int, int> GetDiffDetails(string left, string right)
        {
            var differences = new Dictionary<int, int>();

            int length = 0;
            int offset = -1;

            for (int i = 0; i <= left.Length; i++)
            {
                if (i < left.Length && left[i] != right[i])
                {
                    length++;

                    if (offset < 0)
                    {
                        offset = i;
                    }
                }
                else if (offset != -1)
                {
                    differences[offset] = length;

                    length = 0;
                    offset = 1;
                }
            }

            return differences;
        }

        public DiffInput AddInput(Diff diff, DiffDirection direction, string data)
        {
            var input = new DiffInput(direction, data);

            if (diff.Inputs == null)
            {
                diff.Inputs = new List<DiffInput>();
            }

            diff.Inputs.Add(input);

            return input;
        }

        private static DiffInput GetLastInput(Diff diff, DiffDirection direction)
        {
            return diff.Inputs?.LastOrDefault(_ => _.Direction == direction);
        }
    }
}
