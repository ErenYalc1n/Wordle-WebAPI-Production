namespace Wordle.Application.Users.DTOs
{
    public class AuthResultDto
    {
        public string Token { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }
}
