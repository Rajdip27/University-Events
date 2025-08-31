using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Core.Entities;

public class FoodToken:AuditableEntity
{
    public long RegistrationId { get; set; }
    public StudentRegistration Registration { get; set; }
    public string TokenCode { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);
    public bool IsUsed { get; set; } = false;
    public DateTimeOffset IssuedUtc { get; set; } = DateTimeOffset.UtcNow;
}
