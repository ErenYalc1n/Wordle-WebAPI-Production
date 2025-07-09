namespace Wordle.Application.Users.DTOs
{
    public class TokenResultDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}
