using System;
using System.ComponentModel.DataAnnotations;

namespace FakeTinder.API.Dtos
{
    public class UserForRegisterDto
    {
        public UserForRegisterDto()
        {
            this.Created = DateTime.Now;
            this.LastActive = DateTime.Now;
        }

        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 4, ErrorMessage = "Password must be at least 4 characters long.")]
        public string Password { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string KnownAs { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
    }
}