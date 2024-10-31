using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DonanimAPI.Models
{
    public class DonanimBilgileri
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        //public string Id { get; set; } // MongoDB'de benzersiz bir kimlik

        public string CpuInfo { get; set; } // CPU bilgisi
        public string GpuInfo { get; set; } // GPU bilgisi
        public string RamInfo { get; set; } // RAM bilgisi
        public string DiskInfo { get; set; } // Disk bilgisi

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; } = DateTime.Now; // Tarih ve saat bilgisi
    }
}
