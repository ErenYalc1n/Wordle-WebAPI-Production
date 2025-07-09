namespace Wordle.Application.Users.DTOs
{
    public class LoginResultDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }

}
