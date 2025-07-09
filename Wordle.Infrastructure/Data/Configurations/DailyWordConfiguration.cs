using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wordle.Domain.DailyWords;

namespace Wordle.Infrastructure.Data.Configurations;

public class DailyWordConfiguration : IEntityTypeConfiguration<DailyWord>
{
    public void Configure(EntityTypeBuilder<DailyWord> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Word)
               .IsRequired()
               .HasMaxLength(5);

        builder.Property(e => e.Date)
               .IsRequired()
               .HasColumnType("date");
    }
}
