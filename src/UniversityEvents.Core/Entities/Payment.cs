using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Core.Entities;

public class Payment: AuditableEntity
{
   
    public long RegistrationId { get; set; }
    public StudentRegistration Registration { get; set; }
    public string Provider { get; set; } = "Stripe";
    public string ProviderSessionId { get; set; } = default!;
    public string Status { get; set; } = "Pending";
    public decimal Amount { get; set; }
   
}
