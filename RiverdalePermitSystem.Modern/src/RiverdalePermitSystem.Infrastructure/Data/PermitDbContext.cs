using Microsoft.EntityFrameworkCore;
using RiverdalePermitSystem.Domain.Entities;

namespace RiverdalePermitSystem.Infrastructure.Data;

public class PermitDbContext : DbContext
{
    public PermitDbContext(DbContextOptions<PermitDbContext> options) : base(options) { }

    public DbSet<Permit> Permits => Set<Permit>();
    public DbSet<Applicant> Applicants => Set<Applicant>();
    public DbSet<Contractor> Contractors => Set<Contractor>();
    public DbSet<PlanReview> PlanReviews => Set<PlanReview>();
    public DbSet<Inspection> Inspections => Set<Inspection>();
    public DbSet<Fee> Fees => Set<Fee>();
    public DbSet<ActivityLogEntry> ActivityLog => Set<ActivityLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PermitDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
