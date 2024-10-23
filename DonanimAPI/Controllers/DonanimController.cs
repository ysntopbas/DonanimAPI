using Microsoft.AspNetCore.Mvc;
using DonanimAPI.Models;

namespace DonanimAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonanimController : ControllerBase
    {
        

        [HttpPost]
        public IActionResult PostDonanimBilgileri([FromBody] DonanimBilgileri donanimBilgileri)
        {
            // Gelen donanım bilgilerini işleme
            if (donanimBilgileri == null)
            {
                return BadRequest("Donanım bilgileri geçersiz.");
            }

            // Donanım bilgilerini işleyin (örneğin, bir veritabanına kaydedin)
            // Burada donanimBilgileri nesnesine erişebilirsiniz.
            // Örneğin:
            // var cpuInfo = donanimBilgileri.CpuInfo;
           
            Console.WriteLine(donanimBilgileri.GpuInfo);
            return Ok(); // Başarılı bir yanıt döndür
        }
        
    }
}
