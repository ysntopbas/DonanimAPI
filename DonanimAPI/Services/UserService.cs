using BCrypt.Net;
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

        // Kullanıcıyı kaydetme
        public async Task<User?> RegisterAsync(User user)
        {
            // Kullanıcı adı ve e-posta zaten var mı kontrol et
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

            // Yeni bir ObjectId oluştur ve kullanıcıya ata
            user.Id = ObjectId.GenerateNewId().ToString();
            await _users.InsertOneAsync(user);
            return user;
        }

        // Login işlemi: şifre hash'ini kontrol et
        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }

            // Kullanıcıyı bulduktan sonra şifreyi doğrula
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (isPasswordValid)
            {
                return user;
            }

            return null; // Şifre geçerli değilse null döndür
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        internal async Task<object> Login(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
