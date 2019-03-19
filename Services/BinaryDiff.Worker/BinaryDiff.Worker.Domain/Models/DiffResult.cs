using BinaryDiff.Worker.Domain.Enums;
using System.Collections.Generic;

namespace BinaryDiff.Worker.Domain.Models
{
    public class DiffResult
    {
        public DiffResult(ResultType result, IDictionary<int, int> differences)
        {
            Result = result;
            Differences = differences;
        }

        public ResultType Result { get; set; }

        public IDictionary<int, int> Differences { get; set; }
    }
}
