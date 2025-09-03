using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UniversityEvents.Core.Entities;
using UniversityEvents.Infrastructure.Services;

namespace UniversityEvents.Infrastructure.Data;

public class UniversityDbContext : DbContext
{
    private readonly AuditHelper? _auditHelper;

    // Single constructor, AuditHelper is optional
    public UniversityDbContext(DbContextOptions<UniversityDbContext> options, AuditHelper? auditHelper = null)
        : base(options)
    {
        _auditHelper = auditHelper;
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<StudentRegistration> StudentRegistrations => Set<StudentRegistration>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<FoodToken> FoodTokens => Set<FoodToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Category -> Event (1:n)
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Category)
            .WithMany(c => c.Events)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Event -> StudentRegistration (1:n)
        modelBuilder.Entity<StudentRegistration>()
            .HasOne(s => s.Event)
            .WithMany(e => e.Registrations)
            .HasForeignKey(s => s.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // StudentRegistration -> Payment (1:1)
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Registration)
            .WithOne(s => s.Payment)
            .HasForeignKey<Payment>(p => p.RegistrationId)
            .OnDelete(DeleteBehavior.Cascade);

        // StudentRegistration -> FoodToken (1:1)
        modelBuilder.Entity<FoodToken>()
            .HasOne(f => f.Registration)
            .WithOne(s => s.FoodToken)
            .HasForeignKey<FoodToken>(f => f.RegistrationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraints
        modelBuilder.Entity<Event>().HasIndex(e => e.Slug).IsUnique();
        modelBuilder.Entity<StudentRegistration>().HasIndex(s => s.IdCardNumber).IsUnique();

        base.OnModelCreating(modelBuilder);
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
        if (_auditHelper == null) return; // skip audits when running migrations

        var entries = ChangeTracker.Entries().ToList();
        var audits = _auditHelper.CreateAuditLogs(entries);
        if (audits.Any())
            AuditLogs.AddRange(audits);
    }
}
