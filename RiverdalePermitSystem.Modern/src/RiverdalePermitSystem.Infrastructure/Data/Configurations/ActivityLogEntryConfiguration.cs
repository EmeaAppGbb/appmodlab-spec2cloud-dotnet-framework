using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverdalePermitSystem.Domain.Entities;

namespace RiverdalePermitSystem.Infrastructure.Data.Configurations;

public class ActivityLogEntryConfiguration : IEntityTypeConfiguration<ActivityLogEntry>
{
    public void Configure(EntityTypeBuilder<ActivityLogEntry> builder)
    {
        builder.HasKey(a => a.LogId);
        builder.Property(a => a.ActivityType).HasConversion<string>().HasMaxLength(50);
        builder.Property(a => a.PermitId).HasMaxLength(50);
        builder.Property(a => a.Description).IsRequired().HasMaxLength(500);
        builder.Property(a => a.UserName).IsRequired().HasMaxLength(100);

        builder.HasIndex(a => a.Timestamp);
    }
}
