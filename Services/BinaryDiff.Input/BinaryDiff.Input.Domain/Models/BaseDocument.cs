using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BinaryDiff.Input.Domain.Models
{
    public class BaseDocument
    {
        /// <summary>
        /// Document ID in Mongo
        /// </summary>
        [BsonIgnoreIfNull]
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
