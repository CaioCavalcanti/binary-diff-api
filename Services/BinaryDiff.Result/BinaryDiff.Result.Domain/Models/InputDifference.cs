﻿using System;

namespace BinaryDiff.Result.Domain.Models
{
    public class InputDifference : BaseModel
    {
        public InputDifference()
        {
        }

        public InputDifference(int offset, int length)
        {
            Offset = offset;
            Length = length;
        }

        public Guid ResultId { get; set; }

        public int Offset { get; set; }

        public int Length { get; set; }

        public virtual DiffResult Result { get; set; }
    }
}
