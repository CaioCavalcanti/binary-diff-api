using BinaryDiff.Shared.Domain.Models;
using BinaryDiff.Worker.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BinaryDiff.Worker.Domain.Models
{
    /// <summary>
    /// Entity that holds the values to be compared
    /// </summary>
    [BsonIgnoreExtraElements]
    public class InputData : BaseDocument
    {
        public InputData() { }

        public InputData(InputPosition position, string data)
        {
            Position = position;
            Data = data;
        }

        /// <summary>
        /// Diff unique ID
        /// </summary>
        [BsonElement("diffId")]
        public Guid DiffId { get; set; }

        /// <summary>
        /// Timestamp the document was created
        /// </summary>
        [BsonElement("timestamp")]
        [BsonDateTimeOptions(DateOnly = false, Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Input position
        /// </summary>
        [BsonElement("position")]
        public InputPosition Position { get; set; }

        /// <summary>
        /// Base 64 encoded data
        /// </summary>
        [BsonElement("data")]
        public string Data { get; set; }
    }
}
