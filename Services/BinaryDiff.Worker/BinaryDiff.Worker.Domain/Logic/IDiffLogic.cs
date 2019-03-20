using BinaryDiff.Worker.Domain.Enums;
using BinaryDiff.Worker.Domain.Models;

namespace BinaryDiff.Worker.Domain.Logic
{
    public interface IDiffLogic
    {
        DiffResult CompareData(InputData input, InputData opposingInput);

        InputPosition GetOpposingPosition(InputPosition position);
    }
}
