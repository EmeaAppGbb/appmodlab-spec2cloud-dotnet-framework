using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverdalePermitSystem.Domain.Entities;

namespace RiverdalePermitSystem.Infrastructure.Data.Configurations;

public class ApplicantConfiguration : IEntityTypeConfiguration<Applicant>
{
    public void Configure(EntityTypeBuilder<Applicant> builder)
    {
        builder.HasKey(a => a.ApplicantId);
        builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Email).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Phone).IsRequired().HasMaxLength(20);
        builder.Property(a => a.Company).HasMaxLength(200);
        builder.Property(a => a.LicenseNumber).HasMaxLength(50);

        builder.HasIndex(a => a.Email).IsUnique();
    }
}
