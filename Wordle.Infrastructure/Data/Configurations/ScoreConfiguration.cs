using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wordle.Infrastructure.Data.Configurations;

public class ScoreConfiguration : IEntityTypeConfiguration<Score>
{
    public void Configure(EntityTypeBuilder<Score> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Date)
    .HasConversion(
        v => v.ToDateTime(TimeOnly.MinValue),
        v => DateOnly.FromDateTime(v))
    .IsRequired();



        builder.Property(s => s.Point)
            .IsRequired();

        builder.HasIndex(s => new { s.UserId, s.DailyWordId })
            .IsUnique();

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId);

        builder.HasOne(s => s.DailyWord)
            .WithMany()
            .HasForeignKey(s => s.DailyWordId);
    }
}
