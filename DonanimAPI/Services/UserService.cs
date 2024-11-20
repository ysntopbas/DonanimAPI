using DonanimAPI.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace DonanimAPI.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
        }

        public async Task<User?> RegisterAsync(User user)
        {
            // Yeni bir ObjectId oluştur ve kullanıcıya ata
            user.Id = ObjectId.GenerateNewId().ToString();
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _users.Find(u => u.Username == username && u.Password == password).FirstOrDefaultAsync();
            return user;
        }


        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }
    }
}
