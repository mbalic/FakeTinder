using System.ComponentModel.DataAnnotations;

namespace FakeTinder.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 4, ErrorMessage = "Password must be at least 4 characters long.")]
        public string Password { get; set; }
    }
}