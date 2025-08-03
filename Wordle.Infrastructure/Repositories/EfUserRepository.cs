using Microsoft.EntityFrameworkCore;
using Wordle.Application.Common.Interfaces;
using Wordle.Domain.Users;
using Wordle.Infrastructure.Data;

namespace Wordle.Infrastructure.Repositories
{
    public class EfUserRepository : IUserRepository
    {
        private readonly WordleDbContext _context;

        public EfUserRepository(WordleDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(User user)
        {
            _context.Users.Add(user);
            return Task.CompletedTask;
        }

        public Task<User?> GetByEmailAsync(string email) =>
            _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        public Task<User?> GetByIdAsync(Guid id) =>
            _context.Users.FirstOrDefaultAsync(x => x.Id == id);

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
        }

        public Task<bool> IsEmailConfirmedAsync(string email) =>
            _context.Users.AnyAsync(x => x.Email == email && x.IsEmailConfirmed);

        public Task<bool> IsReminderEmailAllowedAsync(Guid userId) =>
            _context.Users.AnyAsync(x => x.Id == userId && x.IsKvkkAccepted);

        public Task<User?> AuthenticateAsync(string email, string passwordHash) =>
            _context.Users.FirstOrDefaultAsync(x =>
                x.Email == email && x.PasswordHash == passwordHash);

        public Task<User?> GetByNicknameAsync(string nickname) =>
            _context.Users.FirstOrDefaultAsync(x => x.Nickname == nickname);

        public Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public Task<User?> GetByIdentifierAsync(string identifier) =>
            _context.Users.FirstOrDefaultAsync(x =>
                x.Email == identifier || x.Nickname == identifier);

        public Task<User?> GetByRefreshTokenAsync(string refreshToken) =>
            _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        public Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            return Task.CompletedTask;
        }
        public Task<int> CountAsync()
        {
            return _context.Users.CountAsync();
        }

    }
}
