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

        [HttpGet("last/{deviceId}")]
        public async Task<ActionResult<Message>> GetLastMessage(string deviceId)
        {
            try
            {
                var message = await _messageService.GetLastMessageAsync(deviceId);
                if (message == null)
                    return NotFound("Bu cihaz için mesaj bulunamadı.");

                return Ok(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Son mesaj alınamadı: {ex.Message}");
            }
        }
    }
} 