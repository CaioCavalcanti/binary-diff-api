using BinaryDiff.Domain.Models;

namespace BinaryDiff.Domain.Logic
{
    public interface IDiffLogic
    {
        DiffResult GetResultFor(Diff diff);
    }
}
