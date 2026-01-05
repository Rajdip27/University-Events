using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UniversityEvents.Application.SSLCommerz.Models;
using UniversityEvents.Application.SSLCommerz.Validators;

namespace UniversityEvents.Application.SSLCommerz.Services;

public interface ISSLCommerzService
{
    Task<SSLCommerzInitResponse> CreatePaymentAsync(
        SSLCommerzPaymentRequest request,
        CancellationToken cancellationToken = default);

    Task<SSLCommerzValidationResponse> ValidatePaymentAsync(
        string validationId,
        CancellationToken cancellationToken = default);
}
public sealed class SSLCommerzService : ISSLCommerzService
{
    private readonly SSLCommerzSettings _settings;
    private readonly HttpClient _http;

    public SSLCommerzService(
        IOptions<SSLCommerzSettings> options,
        HttpClient httpClient)
    {
        _settings = options.Value;
        _http = httpClient;

        SSLCommerzValidator.ValidateSettings(_settings);
    }
    public async Task<SSLCommerzInitResponse> CreatePaymentAsync(
        SSLCommerzPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        SSLCommerzValidator.ValidatePaymentRequest(request);

        var form = new Dictionary<string, string>
        {
            ["store_id"] = _settings.StoreId,
            ["store_passwd"] = _settings.StorePassword,
            ["total_amount"] = request.Amount.ToString("0.00"),
            ["currency"] = "BDT",
            ["tran_id"] = request.TransactionId,

            ["success_url"] = request.SuccessUrl,
            ["fail_url"] = request.FailUrl,
            ["cancel_url"] = request.CancelUrl,

            ["cus_name"] = request.CustomerName,
            ["cus_email"] = request.CustomerEmail,
            ["cus_phone"] = request.CustomerPhone,

            ["product_name"] = "Online Payment",
            ["product_category"] = "Service",
            ["product_profile"] = "general",
            ["shipping_method"] = "NO"
        };

        var url = $"{_settings.BaseUrl.TrimEnd('/')}/gwprocess/v4/api.php";

        using var response = await _http.PostAsync(
            url,
            new FormUrlEncodedContent(form),
            cancellationToken);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"SSLCommerz Init Error: {json}");

        var result = JsonConvert.DeserializeObject<SSLCommerzInitResponse>(json)
                     ?? throw new InvalidOperationException("Invalid init response.");

        if (!result.Status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(result.FailedReason);

        return result;
    }

    // ================= VALIDATE PAYMENT =================
    public async Task<SSLCommerzValidationResponse> ValidatePaymentAsync(
        string validationId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(validationId))
            throw new ArgumentException("ValidationId is required.");

        var url =
            $"{_settings.BaseUrl.TrimEnd('/')}/validator/api/validationserverAPI.php" +
            $"?val_id={validationId}" +
            $"&store_id={_settings.StoreId}" +
            $"&store_passwd={_settings.StorePassword}" +
            $"&format=json";

        using var response = await _http.GetAsync(url, cancellationToken);

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"SSLCommerz Validation Error: {json}");

        var result = JsonConvert.DeserializeObject<SSLCommerzValidationResponse>(json)
                     ?? throw new InvalidOperationException("Invalid validation response.");

        if (!result.IsPaid)
            throw new InvalidOperationException($"Payment not valid. Status: {result.Status}");

        return result;
    }
}