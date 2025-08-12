using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace YetenekYonetimAPI.Models
{
    public class Application
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("adayId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AdayId { get; set; } = null!;

        [BsonElement("jobPostingId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string JobPostingId { get; set; } = null!;

        [BsonElement("appliedAt")]
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    }
}