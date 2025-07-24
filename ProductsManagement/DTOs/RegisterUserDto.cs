using System.ComponentModel.DataAnnotations;

namespace ProductsManagement.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
