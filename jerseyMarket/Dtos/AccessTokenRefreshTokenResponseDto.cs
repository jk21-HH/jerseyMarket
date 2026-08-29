namespace jerseyMarket.Dtos
{
    public class AccessTokenRefreshTokenResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
