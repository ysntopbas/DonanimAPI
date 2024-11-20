using DonanimAPI.Models;
using DonanimAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IConfiguration _configuration;

    public AuthController(UserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    // Register User
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        // Model doğrulaması
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Hatalı model durumunda 400 döner
        }

        var existingUser = await _userService.LoginAsync(model.Username, model.Password);
        if (existingUser != null)
        {
            return BadRequest("User already exists.");
        }

        // Kullanıcıyı kayıt etme
        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            Password = model.Password
        };

        await _userService.RegisterAsync(user);
        return Ok(new { Message = "Registration successful" });
    }

    // Login User
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        // Model doğrulaması
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Hatalı model durumunda 400 döner
        }

        var loggedInUser = await _userService.LoginAsync(model.Username, model.Password);
        if (loggedInUser == null)
            return Unauthorized("Invalid username or password.");

        var token = GenerateJwtToken(loggedInUser);

        return Ok(new { Message = "Login successful", Token = token });
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[] {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, "User") // Opsiyonel, rol ataması yapılabilir
        };

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
