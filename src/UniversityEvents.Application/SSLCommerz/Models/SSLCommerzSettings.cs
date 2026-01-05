namespace UniversityEvents.Application.SSLCommerz.Models;

public sealed  class SSLCommerzSettings
{
    public string StoreId { get; init; } = string.Empty;
    public string StorePassword { get; init; } = string.Empty;
    public string BaseUrl { get; init; } = string.Empty;
    public bool IsValid() =>
        !string.IsNullOrWhiteSpace(StoreId) &&
        !string.IsNullOrWhiteSpace(StorePassword) &&
        Uri.IsWellFormedUriString(BaseUrl, UriKind.Absolute);
}
