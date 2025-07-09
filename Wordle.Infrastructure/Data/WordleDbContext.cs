using Microsoft.EntityFrameworkCore;
using Wordle.Domain.Users;
using Wordle.Domain.DailyWords;
using Wordle.Domain.Guesses;

namespace Wordle.Infrastructure.Data
{
    public class WordleDbContext : DbContext
    {
        public WordleDbContext(DbContextOptions<WordleDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<DailyWord> DailyWords => Set<DailyWord>();
        public DbSet<Guess> Guesses => Set<Guess>();
        public DbSet<Score> Scores { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(Guid) || property.ClrType == typeof(Guid?))
                    {
                        property.SetColumnType("uuid");
                    }
                }
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WordleDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
