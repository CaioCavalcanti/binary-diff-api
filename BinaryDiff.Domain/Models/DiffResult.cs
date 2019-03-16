using BinaryDiff.Domain.Enum;
using System;
using System.Collections.Generic;

namespace BinaryDiff.Domain.Models
{
    public class DiffResult
    {
        public Guid Id { get; set; }

        public ResultType Result { get; set; }

        public IDictionary<int, int> Differences { get; set; }
    }
}
