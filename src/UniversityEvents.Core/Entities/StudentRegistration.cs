using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Core.Entities;

public class StudentRegistration: AuditableEntity
{
    public long EventId { get; set; }
    public Event Event { get; set; }
    public string FullName { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string IdCardNumber { get; set; } = default!;
    public string Department { get; set; } = default!; 
    public string PhotoPath { get; set; } = default!;
    // PaymentStatus can be "Pending", "Completed", "Failed"
    public string PaymentStatus { get; set; } = "Pending";
    public ICollection<Payment> Payment { get; set; }
    public ICollection<FoodToken> FoodToken { get; set; }
}
