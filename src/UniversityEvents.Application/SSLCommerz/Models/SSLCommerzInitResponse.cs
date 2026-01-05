using Newtonsoft.Json;

namespace UniversityEvents.Application.SSLCommerz.Models;

public class SSLCommerzInitResponse
{
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("failedreason")]
    public string FailedReason { get; set; } = string.Empty;

    [JsonProperty("GatewayPageURL")]
    public string GatewayPageURL { get; set; } = string.Empty;
}
