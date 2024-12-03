using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DonanimAPI.Models
{
    public class UserDevice
    {
        public ObjectId Id { get; set; }


        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string DeviceID { get; set; }= string.Empty;

        
    }
}
