using BinaryDiff.Domain.Models;
using System.Threading.Tasks;

namespace BinaryDiff.Domain.Logic
{
    public interface IDiffLogic
    {
        Task GetDiffResult(string left, string right);
    }
}
