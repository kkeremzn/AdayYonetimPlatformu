using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace YetenekYonetimAPI.Models
{
    public class JobPosting
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("companyId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CompanyId { get; set; } = null!;

        [BsonElement("title")]
        public string Title { get; set; } = null!;

        [BsonElement("description")]
        public string Description { get; set; } = null!;

        [BsonElement("location")]
        public string Location { get; set; } = null!;

        [BsonElement("isPublished")]
        public bool IsPublished { get; set; } = false;

        [BsonElement("postedAt")]
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("assignedToUserId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? AssignedToUserId { get; set; }
    }
}