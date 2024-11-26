using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DonanimAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty; // MongoDB'den otomatik gelir

        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;



    }
}
