using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverdalePermitSystem.Domain.Entities;

namespace RiverdalePermitSystem.Infrastructure.Data.Configurations;

public class InspectionConfiguration : IEntityTypeConfiguration<Inspection>
{
    public void Configure(EntityTypeBuilder<Inspection> builder)
    {
        builder.HasKey(i => i.InspectionId);
        builder.Property(i => i.InspectionId).HasMaxLength(50);
        builder.Property(i => i.PermitId).IsRequired().HasMaxLength(50);
        builder.Property(i => i.InspectorId).IsRequired().HasMaxLength(100);
        builder.Property(i => i.InspectionType).HasConversion<string>().HasMaxLength(50);
        builder.Property(i => i.Result).HasConversion<string>().HasMaxLength(20);
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(i => i.Comments).HasMaxLength(4000);
        builder.Property(i => i.Photos).HasMaxLength(4000);

        builder.HasOne(i => i.Permit).WithMany(p => p.Inspections).HasForeignKey(i => i.PermitId);
        builder.HasIndex(i => i.ScheduledDate);
    }
}
