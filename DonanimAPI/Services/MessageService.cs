using DonanimAPI.Models;
using MongoDB.Driver;

namespace DonanimAPI.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMongoCollection<Message> _messages;

        public MessageService(IMongoDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database), "MongoDB database connection is not initialized.");
            }
            _messages = database.GetCollection<Message>("Messages");
        }

        public async Task<Message> CreateMessageAsync(Message message)
        {
            message.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            message.MessageDate = DateTime.UtcNow;
            await _messages.InsertOneAsync(message);
            return message;
        }

        public async Task<Message> GetLastMessageAsync(string deviceId)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.DeviceID, deviceId);
            var sort = Builders<Message>.Sort.Descending(m => m.MessageDate);

            return await _messages.Find(filter)
                                .Sort(sort)
                                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesByDeviceIdAsync(string deviceId)
        {
            return await _messages
                .Find(m => m.DeviceID == deviceId)
                .SortByDescending(m => m.MessageDate)
                .ToListAsync();
        }
    }
} 