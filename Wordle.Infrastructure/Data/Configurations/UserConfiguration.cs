using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wordle.Domain.Users;

namespace Wordle.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Email)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(e => e.PasswordHash)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(e => e.Nickname)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(e => e.FirstName)
               .HasMaxLength(50);

        builder.Property(e => e.LastName)
               .HasMaxLength(50);

        builder.Property(e => e.IsEmailConfirmed)
               .IsRequired();

        builder.Property(e => e.IsKvkkAccepted)
               .IsRequired();

        builder.Property(e => e.Role)
               .IsRequired();
    }
}
