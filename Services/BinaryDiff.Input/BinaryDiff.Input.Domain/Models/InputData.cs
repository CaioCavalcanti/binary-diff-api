using BinaryDiff.Input.Domain.Enums;
using BinaryDiff.Shared.Domain.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BinaryDiff.Input.Domain.Models
{
    /// <summary>
    /// Entity that holds the values to be compared
    /// </summary>
    [BsonIgnoreExtraElements]
    public class InputData : BaseDocument
    {
        public InputData(Guid diff, InputPosition position, string data)
        {
            DiffId = diff;
            Position = position;
            Data = data;
            Timestamp = DateTime.UtcNow;
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
