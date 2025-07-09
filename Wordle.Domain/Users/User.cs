namespace Wordle.Domain.Users
{
    public sealed class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string Nickname { get; set; } = default!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Role Role { get; set; } = Role.Player;
        public bool IsEmailConfirmed { get; set; }
        public bool IsKvkkAccepted { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
        public string? EmailVerificationCode { get; set; }
        public DateTime? EmailVerificationExpiresAt { get; set; }
        public string? PasswordResetCode { get; set; }
        public DateTime? PasswordResetExpiresAt { get; set; }

    }
}
