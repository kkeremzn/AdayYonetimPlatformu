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
        public string? Sifre { get; set; } 

        [BsonElement("rol")]
        public string? Rol { get; set; } 

        [BsonElement("sirketId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? SirketId { get; set; } 

        internal void ForEach(Func<object, object> value)
        {
            throw new NotImplementedException();
        }
    }
}