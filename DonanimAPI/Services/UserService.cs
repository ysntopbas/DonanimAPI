using BCrypt.Net;
using DonanimAPI.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace DonanimAPI.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<UserDevice> _userDevices;

        public UserService(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
            _userDevices = database.GetCollection<UserDevice>("UsersDevices");
        }

        public async Task<User?> RegisterAsync(User user)
        {
            var existingUserByUsername = await _users.Find(u => u.Username == user.Username).FirstOrDefaultAsync();
            if (existingUserByUsername != null)
            {
                throw new Exception("Username already exists.");
            }

            var existingUserByEmail = await _users.Find(u => u.Email == user.Email).FirstOrDefaultAsync();
            if (existingUserByEmail != null)
            {
                throw new Exception("Email already exists.");
            }

            user.Id = ObjectId.GenerateNewId().ToString();
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
            if (user == null) return null;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (isPasswordValid) return user;

            return null;
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddUserDeviceAsync(string username, string deviceID)
        {
            var exists = await _userDevices.Find(ud => ud.Username == username && ud.DeviceID == deviceID).FirstOrDefaultAsync();
            if (exists != null)
            {
                throw new Exception("This device is already registered for the user.");
            }

            var userDevice = new UserDevice
            {
                Username = username,
                DeviceID = deviceID
            };

            await _userDevices.InsertOneAsync(userDevice);
        }

        public async Task<List<string>> GetUserDevicesAsync(string username)
        {
            var devices = await _userDevices.Find(ud => ud.Username == username).ToListAsync();
            return devices.Select(d => d.DeviceID).ToList();
        }

        // Yeni DeleteUserDeviceAsync metodu ekledik
        public async Task DeleteUserDeviceAsync(string username, string deviceID)
        {
            var result = await _userDevices.DeleteOneAsync(ud => ud.Username == username && ud.DeviceID == deviceID);
            if (result.DeletedCount == 0)
            {
                throw new Exception("Device not found for the user.");
            }
        }
    }
}
