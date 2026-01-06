using System.ComponentModel.DataAnnotations.Schema;
using UniversityEvents.Core.Entities.BaseEntities;

namespace UniversityEvents.Core.Entities;

public class PaymentHistory :AuditableEntity
{
    public long PaymentId { get; set; }
    public Payment Payment { get; set; } = default!;
    public string Provider { get; set; } = "SSLCommerz";
    public string ProviderSessionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;         
    public string TransactionId { get; set; } = string.Empty;    
    public string ValidationId { get; set; } = string.Empty;     
    public string BankName { get; set; } = string.Empty;   
    public string CustomerName { get; set; } = string.Empty;    
    public string CustomerMobile { get; set; } = string.Empty;  
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; } = 0;                    
    public string Currency { get; set; } = string.Empty;
    public string JsonResponse { get; set; } = string.Empty;
    [NotMapped]
    public bool IsPaid =>
        Status.Equals("VALID", StringComparison.OrdinalIgnoreCase) ||
        Status.Equals("VALIDATED", StringComparison.OrdinalIgnoreCase);
}
