using jerseyMarket.Dtos;
using jerseyMarket.Enums;

namespace jerseyMarket.Services
{
    public interface IAuthService
    {
        Task<(RegisterResult Result, UserResponseDto? User)> RegisterAsync(UserRegisterRequestDto request);
        Task<AccessTokenRefreshTokenResponseDto?> LoginAsync(UserLoginRequestDto request);
        Task<AccessTokenRefreshTokenResponseDto?> RegenerateAccessTokenRefreshTokenAsync(RefreshTokenRequestDto request);
        Task LogoutAsync(int userId);
    }
}
