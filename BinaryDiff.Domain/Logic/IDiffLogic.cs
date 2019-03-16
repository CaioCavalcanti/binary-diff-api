using BinaryDiff.Domain.Enum;
using BinaryDiff.Domain.Models;
using System.Collections.Generic;

namespace BinaryDiff.Domain.Logic
{
    public interface IDiffLogic
    {
        DiffType GetResultFor(Diff diff, out Dictionary<int, int> differences);

        DiffInput AddInput(Diff diff, DiffDirection direction, string data);
    }
}
