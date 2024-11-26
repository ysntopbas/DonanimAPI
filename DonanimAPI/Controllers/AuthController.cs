using DonanimAPI.Models;
using DonanimAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
            return BadRequest(new { Message = "Invalid model." });
        }

        try
        {
            var existingUser = await _userService.LoginAsync(model.Username, model.Password);
            if (existingUser != null)
            {
                return BadRequest(new { Message = "User already exists." });
            }

            // Kullanıcıyı kayıt etme
            var user = new User
            {
                Username = model.Username,
                Email = model.Email
            };

            // Şifreyi hash'le
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

            await _userService.RegisterAsync(user);
            return Ok(new { Message = "Registration successful" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred during registration", Error = ex.Message });
        }
    }

    // Login User
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        // Model doğrulaması
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
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
            new Claim(ClaimTypes.Role, "User")
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
