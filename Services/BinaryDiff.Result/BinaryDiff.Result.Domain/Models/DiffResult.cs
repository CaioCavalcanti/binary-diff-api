using BinaryDiff.Result.Domain.Enums;
using System;
using System.Collections.Generic;

namespace BinaryDiff.Result.Domain.Models
{
    public class DiffResult : BaseModel
    {
        public Guid DiffId { get; set; }

        public DateTime Timestamp { get; set; }

        public ResultType Result { get; set; }

        public virtual ICollection<InputDifference> Differences { get; set; }
    }
}
