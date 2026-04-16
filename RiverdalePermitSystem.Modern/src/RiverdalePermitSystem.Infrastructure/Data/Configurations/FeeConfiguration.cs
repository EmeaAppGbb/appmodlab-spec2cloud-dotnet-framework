using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverdalePermitSystem.Domain.Entities;

namespace RiverdalePermitSystem.Infrastructure.Data.Configurations;

public class FeeConfiguration : IEntityTypeConfiguration<Fee>
{
    public void Configure(EntityTypeBuilder<Fee> builder)
    {
        builder.HasKey(f => f.FeeId);
        builder.Property(f => f.PermitId).IsRequired().HasMaxLength(50);
        builder.Property(f => f.FeeType).IsRequired().HasMaxLength(50);
        builder.Property(f => f.Amount).HasColumnType("decimal(18,2)");
        builder.Property(f => f.PaymentMethod).HasMaxLength(50);
        builder.Property(f => f.TransactionId).HasMaxLength(100);

        builder.HasOne(f => f.Permit).WithMany(p => p.Fees).HasForeignKey(f => f.PermitId);
    }
}
