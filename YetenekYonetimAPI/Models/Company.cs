using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace YetenekYonetimAPI.Models
{
    public class Company
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("address")]
        public string Address { get; set; } = null!;

        [BsonElement("phone")]
        public string Phone { get; set; } = null!;

        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("website")]
        public string Website { get; set; } = null!;

        [BsonElement("description")]
        public string Description { get; set; } = null!;
    }
}