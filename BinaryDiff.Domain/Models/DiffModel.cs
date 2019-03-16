using System;

namespace BinaryDiff.Domain.Models
{
    public class DiffModel
    {
        public DiffModel()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string Left { get; set; }

        public string Right { get; set; }
    }
}
