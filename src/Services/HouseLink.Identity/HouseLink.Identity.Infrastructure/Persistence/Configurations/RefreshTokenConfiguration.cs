using HouseLink.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace HouseLink.Identity.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Token)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(t => t.ReplacedBy)
                .HasMaxLength(512);

            builder.HasIndex(t => t.Token)
                .IsUnique();

            builder.HasIndex(t => t.UserId);

            // Ánh xạ backing fields của private setters
            builder.Property(t => t.UserId).HasColumnName("UserId");
            builder.Property(t => t.ExpiresAt).HasColumnName("ExpiresAt");
            builder.Property(t => t.CreatedAt).HasColumnName("CreatedAt");
            builder.Property(t => t.IsRevoked).HasColumnName("IsRevoked");
            builder.Property(t => t.ReplacedBy).HasColumnName("ReplacedBy");
        }
    }
}
