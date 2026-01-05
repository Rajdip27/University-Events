using Newtonsoft.Json;

namespace UniversityEvents.Application.SSLCommerz.Models;

public class SSLCommerzValidationResponse
{
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("tran_id")]
    public string TransactionId { get; set; } = string.Empty;

    [JsonProperty("val_id")]
    public string ValidationId { get; set; } = string.Empty;

    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonProperty("store_amount")]
    public decimal StoreAmount { get; set; }

    public bool IsPaid =>
        Status.Equals("VALID", StringComparison.OrdinalIgnoreCase) ||
        Status.Equals("VALIDATED", StringComparison.OrdinalIgnoreCase);
}
