using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UniversityEvents.Core.Entities;

namespace UniversityEvents.Infrastructure.Services;

public class AuditHelper
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public AuditHelper(IHttpContextAccessor? httpContextAccessor = null)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public List<AuditLog> CreateAuditLogs(IEnumerable<EntityEntry> entries)
    {
        var audits = new List<AuditLog>();

        foreach (var entry in entries)
        {
            if (entry.State != Microsoft.EntityFrameworkCore.EntityState.Added &&
                entry.State != Microsoft.EntityFrameworkCore.EntityState.Modified &&
                entry.State != Microsoft.EntityFrameworkCore.EntityState.Deleted)
                continue;

            var audit = new AuditLog
            {
                TableName = entry.Entity.GetType().Name,
                Action = entry.State.ToString(),
                KeyValues = JsonSerializer.Serialize(GetPrimaryKey(entry)),
                ChangedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System",
                ChangedAt = DateTime.UtcNow
            };

            if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Modified)
            {
                audit.OldValues = JsonSerializer.Serialize(GetPropertyValues(entry.OriginalValues));
                audit.NewValues = JsonSerializer.Serialize(GetPropertyValues(entry.CurrentValues));
            }
            else if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Added)
            {
                audit.NewValues = JsonSerializer.Serialize(GetPropertyValues(entry.CurrentValues));
            }
            else if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Deleted)
            {
                audit.OldValues = JsonSerializer.Serialize(GetPropertyValues(entry.OriginalValues));
            }

            audits.Add(audit);
        }

        return audits;
    }

    private object GetPrimaryKey(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        var dict = new Dictionary<string, object?>();
        foreach (var prop in key!.Properties)
            dict[prop.Name] = entry.Property(prop.Name).CurrentValue;
        return dict;
    }

    private Dictionary<string, object?> GetPropertyValues(PropertyValues values)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var prop in values.Properties)
            dict[prop.Name] = values[prop.Name];
        return dict;
    }
}
