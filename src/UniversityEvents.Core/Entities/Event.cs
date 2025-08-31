using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Core.Entities;

public class Event: AuditableEntity
{
    public long CategoryId { get; set; }
    public Category Category { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; }
    public DateTimeOffset StartDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset EndDate { get; set; } = DateTimeOffset.UtcNow;
    public decimal RegistrationFee { get; set; }
    public string Slug { get; set; } = default!; 
    public ICollection<StudentRegistration> Registrations { get; set; } = new List<StudentRegistration>();
}
