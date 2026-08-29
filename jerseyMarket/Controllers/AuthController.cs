using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using jerseyMarket.Dtos;
using jerseyMarket.Enums;
using jerseyMarket.Services;

namespace jerseyMarket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(UserRegisterRequestDto request)
        {
            var (result, user) = await authService.RegisterAsync(request);

            return result switch
            {
                RegisterResult.Success => Ok(user),
                RegisterResult.UsernameTaken => Conflict("Username is already taken."),
                _ => StatusCode(500) // unreachable unless the enum grows and a case is missed
            };
        }

        [EnableRateLimiting("login")]
        [HttpPost("login")]
        public async Task<ActionResult<AccessTokenRefreshTokenResponseDto>> Login(UserLoginRequestDto request)
        {
            var res = await authService.LoginAsync(request);

            if (res == null)
            {
                return BadRequest("Invalid username or password.");
            }

            return Ok(res);
        }

        [HttpPost("regenerate-tokens")]
        public async Task<ActionResult<AccessTokenRefreshTokenResponseDto>> RegenerateAccessTokenRefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var res = await authService.RegenerateAccessTokenRefreshTokenAsync(request);

            if (res == null)
            {
                return Unauthorized("Invalid refresh token.");
            }

            return Ok(res);
        }
    }
}
