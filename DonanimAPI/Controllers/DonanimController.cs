using Microsoft.AspNetCore.Mvc;
using DonanimAPI.Models;  // NoteModel sınıfının doğru namespace'ini kullan
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace DonanimAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonanimController : ControllerBase
    {
        private readonly IMongoCollection<DonanimBilgileri> _donanimBilgileriCollection;
        private readonly IMongoCollection<NoteModel> _noteCollection;

        // MongoDB bağlantısını DI ile al
        public DonanimController(IMongoDatabase database)
        {
            _donanimBilgileriCollection = database.GetCollection<DonanimBilgileri>("DonanimBilgileri");
            _noteCollection = database.GetCollection<NoteModel>("Notes");  // Notes koleksiyonunu al
        }

        // Donanım bilgileri ekleme veya güncelleme
        [HttpPost]
        public async Task<IActionResult> PostDonanimBilgileri([FromBody] DonanimBilgileri donanimBilgileri)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Model validasyonu yapılır.
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

                var options = new UpdateOptions { IsUpsert = true }; // Update veya Insert işlemi yapılacak
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

        // Not ekleme (MongoDB'ye kaydetme)
        [HttpPost("SaveNote")]
        public async Task<IActionResult> SaveNote([FromBody] NoteModel noteModel)
        {
            if (noteModel == null)
            {
                return BadRequest("Geçersiz not verisi.");
            }

            try
            {
                // Aynı DeviceID'ye sahip bir not var mı kontrol et
                var filter = Builders<NoteModel>.Filter.Eq(n => n.DeviceID, noteModel.DeviceID);

                // Eğer varsa, güncelleme yap
                var update = Builders<NoteModel>.Update
                    .Set(n => n.Note, noteModel.Note)  // Not içeriğini güncelle
                    .Set(n => n.DateCreated, DateTime.Now);  // Oluşturulma tarihini güncelle

                var result = await _noteCollection.UpdateOneAsync(filter, update);

                if (result.MatchedCount > 0)
                {
                    return Ok("Not başarıyla güncellendi.");
                }

                // Aynı DeviceID ile not bulunamazsa, yeni bir not ekle
                await _noteCollection.InsertOneAsync(noteModel);
                return Ok("Yeni not başarıyla kaydedildi.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Veritabanına not eklenirken bir hata oluştu: {ex.Message}");
            }
        }

        [HttpGet("GetNote/{deviceID}")]
        public async Task<IActionResult> GetNote(string deviceID)
        {
            try
            {
                // DeviceID'ye sahip notu bul
                var filter = Builders<NoteModel>.Filter.Eq(n => n.DeviceID, deviceID);
                var note = await _noteCollection.Find(filter).FirstOrDefaultAsync();

                if (note == null)
                {
                    return NotFound("Not bulunamadı.");
                }

                return Ok(note.Note); // Yalnızca not içeriğini döndür
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Not alınırken bir hata oluştu: {ex.Message}");
            }
        }
    }
}