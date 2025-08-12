using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace YetenekYonetimAPI.Models
{
    public class Aday
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
       
        public string? Id { get; set; }

        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Eposta { get; set; }
        public string Telefon { get; set; }
        public string UzmanlıkAlani { get; set; }


    }
}