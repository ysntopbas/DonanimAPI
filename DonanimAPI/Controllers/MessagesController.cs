using Microsoft.AspNetCore.Mvc;
using DonanimAPI.Models;
using DonanimAPI.Services;

namespace DonanimAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateMessage([FromBody] Message message)
        {
            try
            {
                if (message == null)
                    return BadRequest("Mesaj içeriği boş olamaz.");

                await _messageService.CreateMessageAsync(message);
                return Ok("Mesaj başarıyla gönderildi.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Mesaj gönderilemedi: {ex.Message}");
            }
        }
        

        [HttpGet("{deviceId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessagesByDeviceId(string deviceId)
        {
            try
            {
                var messages = await _messageService.GetMessagesByDeviceIdAsync(deviceId);
                if (!messages.Any())
                {
                    return NotFound("Bu cihaz için mesaj bulunamadı.");
                }

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Mesajlar alınamadı: {ex.Message}");
            }
        }

        [HttpDelete("{deviceId}")]
        public async Task<ActionResult> DeleteMessagesByDeviceId(string deviceId)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceId))
                    return BadRequest("Cihaz ID boş olamaz.");

                var result = await _messageService.DeleteMessagesByDeviceIdAsync(deviceId);
                if (result)
                {
                    return Ok($"{deviceId} cihazına ait tüm mesajlar başarıyla silindi.");
                }
                else
                {
                    return NotFound($"{deviceId} cihazına ait mesaj bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Mesajlar silinirken bir hata oluştu: {ex.Message}");
            }
        }
    }
} 