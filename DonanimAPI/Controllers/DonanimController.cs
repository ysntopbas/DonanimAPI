using Microsoft.AspNetCore.Mvc;
using DonanimAPI.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace DonanimAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonanimController : ControllerBase
    {
        private readonly IMongoCollection<DonanimBilgileri> _donanimBilgileriCollection;

        public DonanimController(IConfiguration config)
        {
            var mongoClient = new MongoClient(config.GetConnectionString("MongoDb"));
            var database = mongoClient.GetDatabase("DonanimDB"); // Veritabanı adını değiştirin
            _donanimBilgileriCollection = database.GetCollection<DonanimBilgileri>("DonanimBilgileri");
        }

        [HttpPost]
        public async Task<IActionResult> PostDonanimBilgileri([FromBody] DonanimBilgileri donanimBilgileri)
        {
            if (donanimBilgileri == null)
            {
                return BadRequest("Donanım bilgileri geçersiz.");
            }

            try
            {
                // Veritabanında DeviceID'ye göre kontrol et
                var filter = Builders<DonanimBilgileri>.Filter.Eq(d => d.DeviceID, donanimBilgileri.DeviceID);

                // Güncelleme için Update işlemi
                var update = Builders<DonanimBilgileri>.Update
                    .Set(d => d.CpuInfos, donanimBilgileri.CpuInfos)
                    .Set(d => d.GpuInfos, donanimBilgileri.GpuInfos)
                    .Set(d => d.RamInfo, donanimBilgileri.RamInfo)
                    .Set(d => d.DiskInfo, donanimBilgileri.DiskInfo)
                    .Set(d => d.Date, donanimBilgileri.Date)
                    .Set(d => d.DeviceName, donanimBilgileri.DeviceName);

                // Upsert işlemi için UpdateOptions
                var options = new UpdateOptions { IsUpsert = true };

                // DeviceID'ye göre upsert işlemini yap
                await _donanimBilgileriCollection.UpdateOneAsync(filter, update, options);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Veritabanına eklenirken bir hata oluştu: {ex.Message}");
            }

            return Ok(); // Başarılı bir yanıt döndür
        }
    }
}
