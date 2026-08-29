using Microsoft.EntityFrameworkCore;

namespace jerseyMarket.Models
{
    [Index(nameof(Username), IsUnique = true)]
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
