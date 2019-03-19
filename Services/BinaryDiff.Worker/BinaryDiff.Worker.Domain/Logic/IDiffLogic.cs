using BinaryDiff.Worker.Domain.Enums;
using BinaryDiff.Worker.Domain.Models;

namespace BinaryDiff.Worker.Domain.Logic
{
    public interface IDiffLogic
    {
        DiffResult CompareData(string left, string right);

        InputPosition GetOpposingPosition(InputPosition position);
    }
}
