using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

using jerseyMarket.Data;
using jerseyMarket.Dtos;
using jerseyMarket.Enums;
using jerseyMarket.Models;


namespace jerseyMarket.Services
{
    public class AuthService(AppDbContext _context, IConfiguration configuration) : IAuthService
    {
        public async Task<(RegisterResult Result, UserResponseDto? User)> RegisterAsync(UserRegisterRequestDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return (RegisterResult.UsernameTaken, null);
            }

            var user = new User();

            var hashedPasswords = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            user.Username = request.Username;
            user.Password = hashedPasswords;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return (RegisterResult.Success, new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username
            });
        }

        public async Task<AccessTokenRefreshTokenResponseDto?> LoginAsync(UserLoginRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                return null;
            }

            var passwordVerificationResult = new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.Password, request.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return await BuildAccessTokenRefreshTokenResponseAsync(user);
        }

        public async Task<AccessTokenRefreshTokenResponseDto?> RegenerateAccessTokenRefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);

            if (user == null)
            {
                return null;
            }

            return await BuildAccessTokenRefreshTokenResponseAsync(user);
        }

        public async Task LogoutAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return;
            }

            RevokeSessions(user);
            await _context.SaveChangesAsync();
        }

        private async Task<AccessTokenRefreshTokenResponseDto?> BuildAccessTokenRefreshTokenResponseAsync(User user)
        {
            // generate a new access token and refresh token for the user
            return new AccessTokenRefreshTokenResponseDto
            {
                AccessToken = GenerateAccessToken(user),
                RefreshToken = await GenerateRefreshTokenAsync(user)
            };
        }

        private string GenerateAccessToken(User user)
        {
            // short claim names so the token stays compact and the payload is easy to read
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.NameId, user.UserId.ToString()),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
        private async Task<string> GenerateRefreshTokenAsync(User user)
        {
            // generate a random refresh token
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            var refreshToken = Convert.ToBase64String(randomNumber);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            user.LastLogin = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return refreshToken;
        }

        private async Task<User?> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            // validate the refresh token for the user
            var user = await _context.Users.FindAsync(userId);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }

        private static void RevokeSessions(User user)
        {
            // nullify the refresh token and set its expiry time to the past - cause logout
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;
        }
    }
}
