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
                await _donanimBilgileriCollection.InsertOneAsync(donanimBilgileri);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Veritabanına eklenirken bir hata oluştu: {ex.Message}");
            }

            return Ok(); // Başarılı bir yanıt döndür
        }
    }
}
