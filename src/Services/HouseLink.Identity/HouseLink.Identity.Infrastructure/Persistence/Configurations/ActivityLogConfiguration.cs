using HouseLink.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HouseLink.Identity.Infrastructure.Persistence.Configurations
{
    public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            builder.ToTable("ActivityLogs");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.UserId).IsRequired();
            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.ActivityType).IsRequired();
            builder.HasIndex(x => x.ActivityType);

            builder.Property(x => x.Description).IsRequired().HasMaxLength(500);

            builder.Property(x => x.IpAddress).HasMaxLength(45);
            builder.HasIndex(x => x.IpAddress);

            builder.Property(x => x.UserAgent).HasMaxLength(500);

            builder.Property(x => x.AdditionalData).HasColumnType("nvarchar(max)");

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
