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

        public List<CpuInfo> CpuInfos { get; set; } = new List<CpuInfo>();
        public List<GpuInfo> GpuInfos { get; set; } = new List<GpuInfo>();
        public Dictionary<string, float> RamInfo { get; set; } = new Dictionary<string, float>(); // RAM bilgileri
        public Dictionary<string, float> DiskInfo { get; set; } = new Dictionary<string, float>(); // Disk bilgileri
       
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; } = DateTime.Now; // Tarih ve saat bilgisi

    }
}
