using System.ComponentModel.DataAnnotations;

namespace jerseyMarket.Dtos
{
    public class UserRegisterRequestDto
    {
        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}
