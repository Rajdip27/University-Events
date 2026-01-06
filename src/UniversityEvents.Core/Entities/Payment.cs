using System.ComponentModel.DataAnnotations.Schema;
using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Core.Entities;

public class Payment: AuditableEntity
{
    public string InvoiceNumber { get; set; } = default!;
    public long RegistrationId { get; set; }
    public StudentRegistration Registration { get; set; }
    public string Provider { get; set; } = "Stripe";
    public string ProviderSessionId { get; set; } = default!;
    public string Status { get; set; } = "Pending";
    public string TransactionId { get; set; } = string.Empty;
    public string ValidationId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    [NotMapped]
    public bool IsPaid =>
       Status.Equals("VALID", StringComparison.OrdinalIgnoreCase) ||
       Status.Equals("VALIDATED", StringComparison.OrdinalIgnoreCase);
    public PaymentHistory  PaymentHistory { get; set; }

}
