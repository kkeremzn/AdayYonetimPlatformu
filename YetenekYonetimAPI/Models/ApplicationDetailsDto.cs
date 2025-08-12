// YetenekYonetimAPI/Models/ApplicationDetailsDto.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace YetenekYonetimAPI.Models
{
    public class AdayDetailsDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string? id { get; set; }

        [BsonElement("ad")]
        [JsonPropertyName("ad")]
        public string ad { get; set; } = null!;

        [BsonElement("soyad")]
        [JsonPropertyName("soyad")]
        public string soyad { get; set; } = null!;

        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string email { get; set; } = null!;
    }

    public class ApplicationDetailsDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string? id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("adayId")]
        [JsonPropertyName("adayId")]
        public string adayId { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("jobPostingId")]
        [JsonPropertyName("jobPostingId")]
        public string jobPostingId { get; set; } = null!;

        [BsonElement("appliedAt")]
        [JsonPropertyName("appliedAt")]
        public DateTime appliedAt { get; set; }

        [BsonElement("adayDetails")]
        [JsonPropertyName("adayDetails")]
        public AdayDetailsDto? adayDetails { get; set; } = null!;
    }
}