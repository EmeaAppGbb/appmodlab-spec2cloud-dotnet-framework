using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverdalePermitSystem.Domain.Entities;

namespace RiverdalePermitSystem.Infrastructure.Data.Configurations;

public class PermitConfiguration : IEntityTypeConfiguration<Permit>
{
    public void Configure(EntityTypeBuilder<Permit> builder)
    {
        builder.HasKey(p => p.PermitId);
        builder.Property(p => p.PermitId).HasMaxLength(50);
        builder.Property(p => p.PropertyAddress).IsRequired().HasMaxLength(200);
        builder.Property(p => p.ParcelNumber).IsRequired().HasMaxLength(50);
        builder.Property(p => p.PermitType).HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.EstimatedCost).HasColumnType("decimal(18,2)");
        builder.Property(p => p.ZoningDistrict).HasConversion<string>().HasMaxLength(10);
        builder.Property(p => p.ProjectDescription).HasMaxLength(4000);
        builder.Property(p => p.CreatedBy).HasMaxLength(100);
        builder.Property(p => p.RowVersion).IsRowVersion();

        builder.HasOne(p => p.Applicant).WithMany(a => a.Permits).HasForeignKey(p => p.ApplicantId);
        builder.HasOne(p => p.Contractor).WithMany(c => c.Permits).HasForeignKey(p => p.ContractorId);

        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.ApplicationDate);
        builder.HasIndex(p => p.PropertyAddress);
    }
}
