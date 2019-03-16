using System;
using System.Collections.Generic;

namespace BinaryDiff.Domain.Models
{
    public class Diff
    {
        public Diff()
        {
            Id = Guid.NewGuid();
            Inputs = new List<DiffInput>();
        }

        public Guid Id { get; set; }

        public IList<DiffInput> Inputs { get; set; }
    }
}
