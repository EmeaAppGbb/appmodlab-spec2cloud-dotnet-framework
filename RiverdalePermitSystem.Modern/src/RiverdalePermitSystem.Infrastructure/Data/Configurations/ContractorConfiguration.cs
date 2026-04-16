using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverdalePermitSystem.Domain.Entities;

namespace RiverdalePermitSystem.Infrastructure.Data.Configurations;

public class ContractorConfiguration : IEntityTypeConfiguration<Contractor>
{
    public void Configure(EntityTypeBuilder<Contractor> builder)
    {
        builder.HasKey(c => c.ContractorId);
        builder.Property(c => c.CompanyName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.LicenseNumber).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Rating).HasColumnType("decimal(3,2)");
        builder.Property(c => c.ContactEmail).IsRequired().HasMaxLength(200);
        builder.Property(c => c.ContactPhone).IsRequired().HasMaxLength(20);

        builder.HasIndex(c => c.LicenseNumber).IsUnique();
    }
}
