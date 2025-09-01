using System.ComponentModel.DataAnnotations;
using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Core.Entities;

public class AuditLog: BaseEntity
{

    [Required]
    public string TableName { get; set; } = default!; 
    [Required]
    public string Action { get; set; } = default!; 
    [Required]
    public string KeyValues { get; set; } = default!; 
    public string OldValues { get; set; } 
    public string NewValues { get; set; } 
    public string ChangedBy { get; set; } 
    public long CreatedBy { get; set; } 
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
