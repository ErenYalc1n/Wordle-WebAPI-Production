using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wordle.Domain.Guesses;

namespace Wordle.Infrastructure.Data.Configurations;

public class GuessConfiguration : IEntityTypeConfiguration<Guess>
{
    public void Configure(EntityTypeBuilder<Guess> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.GuessText)
               .IsRequired()
               .HasMaxLength(5);

        builder.Property(x => x.GuessedAt)
               .IsRequired();

        builder.Property(x => x.IsCorrect)
               .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.DailyWordId });

    }
}
