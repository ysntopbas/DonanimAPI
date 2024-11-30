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

        // Veritabanı bağlantısını DI ile al
        public DonanimController(IMongoDatabase database)
        {
            _donanimBilgileriCollection = database.GetCollection<DonanimBilgileri>("DonanimBilgileri");
        }



        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> PostDonanimBilgileri([FromBody] DonanimBilgileri donanimBilgileri)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // Model validasyonu yapılır.
            }




            try
            {
                var filter = Builders<DonanimBilgileri>.Filter.Eq(d => d.DeviceID, donanimBilgileri.DeviceID);

                var update = Builders<DonanimBilgileri>.Update
                    .Set(d => d.CpuInfos, donanimBilgileri.CpuInfos)
                    .Set(d => d.GpuInfos, donanimBilgileri.GpuInfos)
                    .Set(d => d.RamInfo, donanimBilgileri.RamInfo)
                    .Set(d => d.DiskInfo, donanimBilgileri.DiskInfo)
                    .Set(d => d.Date, donanimBilgileri.Date)
                    .Set(d => d.DeviceName, donanimBilgileri.DeviceName);

                var options = new UpdateOptions { IsUpsert = true };

                var result = await _donanimBilgileriCollection.UpdateOneAsync(filter, update, options);

                if (result.ModifiedCount == 0 && result.UpsertedId == null)
                {
                    return NotFound("Veritabanında güncellenen herhangi bir bilgi bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Veritabanına eklenirken bir hata oluştu: {ex.Message}");
            }

            return Ok("Donanım bilgileri başarıyla güncellendi."); // Başarılı bir yanıt döndür
        }


        [HttpGet("computers")]
        public async Task<IActionResult> GetComputers()
        {
            
            try
            {
                var computers = await _donanimBilgileriCollection.Find(_ => true).ToListAsync();
                return Ok(computers.Select(c => new { c.DeviceID, c.DeviceName, c.Id }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Veri çekme hatası", error = ex.Message });
            }
        }


    }
}