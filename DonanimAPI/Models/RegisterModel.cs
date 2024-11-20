using System.ComponentModel.DataAnnotations;



namespace DonanimAPI.Models
{
    public class RegisterModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)] // Örneğin, minimum 6 karakter
        public string Password { get; set; }
    }
}
