using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;


namespace DonanimAPI.Models
{
    public class DonanimBilgileri
    {

        [BsonId]
        public ObjectId Id { get; set; }  // MongoDB'nin _id alanını karşılamak için ObjectId kullanılır.
        public List<CpuInfo> CpuInfos { get; set; } = new List<CpuInfo>();
        public List<GpuInfo> GpuInfos { get; set; } = new List<GpuInfo>();
        public Dictionary<string, float> RamInfo { get; set; } = new Dictionary<string, float>(); // RAM bilgileri
        public Dictionary<string, float> DiskInfo { get; set; } = new Dictionary<string, float>(); // Disk bilgileri
       
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; } = DateTime.Now; // Tarih ve saat bilgisi
        public required string DeviceID { get; set; }
        public required string DeviceName { get; set; }

    }
}
