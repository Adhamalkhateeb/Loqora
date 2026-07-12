using Loqora.Domain.Identity;
using Loqora.Infrastructure.Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loqora.Infrastructure.Data.Configurations;


public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Id).ValueGeneratedNever();

        builder.Property(rt => rt.Token).HasMaxLength(512).IsRequired();

        builder.HasIndex(rt => rt.Token).IsUnique();

        builder.Property(rt => rt.UserId).IsRequired();

        builder.Property(rt => rt.ExpiresOnUtc).IsRequired();

        builder.Property(rt => rt.RevokedOnUtc);

        builder.Ignore(rt => rt.IsRevoked);

        builder.HasIndex(rt => rt.UserId);

        builder
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
