using Wordle.Domain.Users;

namespace Wordle.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        void Update(User user); 
        void Delete(User user);
        Task<bool> IsReminderEmailAllowedAsync(Guid userId);       
        Task<bool> IsEmailConfirmedAsync(string email);
        Task<User?> AuthenticateAsync(string email, string passwordHash);
        Task<User?> GetByNicknameAsync(string nickname);
        Task UpdateAsync(User user);
        Task<User?> GetByIdentifierAsync(string identifier);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task DeleteAsync(User user);

    }
}
