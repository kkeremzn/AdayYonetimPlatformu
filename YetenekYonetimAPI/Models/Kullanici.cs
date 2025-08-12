using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace YetenekYonetimAPI.Models
{
    public class Kullanici
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("kullaniciAdi")]
        public string? KullaniciAdi { get; set; }

        [BsonElement("sifre")]
        public string? Sifre { get; set; } // Şifreyi hash'leyerek saklayacağız

        [BsonElement("rol")]
        public string? Rol { get; set; } // "IKDirector" veya "IKUzman"

        [BsonElement("sirketId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? SirketId { get; set; } // Şirket ile ilişki kurmak için
    }
}