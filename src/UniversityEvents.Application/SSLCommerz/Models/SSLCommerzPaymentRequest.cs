namespace UniversityEvents.Application.SSLCommerz.Models;

public class SSLCommerzPaymentRequest
{
    public decimal Amount { get; init; }
    public string TransactionId { get; init; } = string.Empty;

    public string SuccessUrl { get; init; } = string.Empty;
    public string FailUrl { get; init; } = string.Empty;
    public string CancelUrl { get; init; } = string.Empty;

    public string CustomerName { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
    public string CustomerPhone { get; init; } = string.Empty;
    public string CustomerAddress { get; init; } = "Dhaka";
    public string CustomerCity { get; init; } = "Dhaka";    
    public string CustomerCountry { get; init; } = "Bangladesh"; 
}
