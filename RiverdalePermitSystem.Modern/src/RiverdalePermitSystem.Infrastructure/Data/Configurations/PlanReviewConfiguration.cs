using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverdalePermitSystem.Domain.Entities;

namespace RiverdalePermitSystem.Infrastructure.Data.Configurations;

public class PlanReviewConfiguration : IEntityTypeConfiguration<PlanReview>
{
    public void Configure(EntityTypeBuilder<PlanReview> builder)
    {
        builder.HasKey(r => r.ReviewId);
        builder.Property(r => r.ReviewId).HasMaxLength(50);
        builder.Property(r => r.PermitId).IsRequired().HasMaxLength(50);
        builder.Property(r => r.ReviewerId).IsRequired().HasMaxLength(100);
        builder.Property(r => r.ReviewType).HasConversion<string>().HasMaxLength(50);
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(r => r.Comments).HasMaxLength(4000);

        // Store deficiencies as semicolon-delimited string
        builder.Property(r => r.Deficiencies)
            .HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(4000);

        builder.HasOne(r => r.Permit).WithMany(p => p.PlanReviews).HasForeignKey(r => r.PermitId);
    }
}
