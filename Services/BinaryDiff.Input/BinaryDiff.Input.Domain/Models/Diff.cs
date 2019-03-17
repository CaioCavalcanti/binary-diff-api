using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BinaryDiff.Input.Domain.Models
{
    public class Diff : BaseDocument
    {
        public Diff()
        {
            UUID = Guid.NewGuid();
        }

        [BsonElement("uuid")]
        public Guid UUID { get; set; }
    }
}
