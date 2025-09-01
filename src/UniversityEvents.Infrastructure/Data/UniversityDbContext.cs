using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UniversityEvents.Core.Entities;
using UniversityEvents.Infrastructure.Services;

namespace UniversityEvents.Infrastructure.Data;

public class UniversityDbContext(DbContextOptions<UniversityDbContext> options, AuditHelper auditHelper) : DbContext(options)
{
    private readonly AuditHelper _auditHelper = auditHelper;

    public DbSet<Category> EventCategories => Set<Category>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<StudentRegistration> StudentRegistrations => Set<StudentRegistration>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<FoodToken> FoodTokens => Set<FoodToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>().HasIndex(e => e.Slug).IsUnique();
        modelBuilder.Entity<StudentRegistration>().HasIndex(e => e.IdCardNumber).IsUnique();
    }
    public override int SaveChanges()
    {
        AddAudits();
        return base.SaveChanges();
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddAudits();
        return base.SaveChangesAsync(cancellationToken);
    }
    private void AddAudits()
    {
        var entries = ChangeTracker.Entries().ToList();
        var audits = _auditHelper.CreateAuditLogs(entries);
        if (audits.Any())
            AuditLogs.AddRange(audits);
    }
}
