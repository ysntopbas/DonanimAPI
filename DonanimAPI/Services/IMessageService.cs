using DonanimAPI.Models;

namespace DonanimAPI.Services
{
    public interface IMessageService
    {
        Task<Message> CreateMessageAsync(Message message);
        Task<Message> GetLastMessageAsync(string deviceId);
        Task<IEnumerable<Message>> GetMessagesByDeviceIdAsync(string deviceId);
    }
} 