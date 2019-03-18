using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BinaryDiff.Shared.Domain.Models
{
    /// <summary>
    /// Represents a base document in mongo db
    /// </summary>
    public class BaseDocument
    {
        [BsonIgnoreIfNull]
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
