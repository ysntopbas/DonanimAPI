using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DonanimAPI.Models
{
    public class NoteModel
    {
        [BsonId] // MongoDB'deki _id alanını belirtir
        public ObjectId Id { get; set; }

        public string DeviceID { get; set; }  // DeviceID'yi tanımlıyoruz
        public string Note { get; set; }  // Not içeriği
        public DateTime DateCreated { get; set; }  // Notun oluşturulma tarihi
    }
}
