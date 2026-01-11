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
    private readonly IHttpClientFactory _httpClientFactory;

    public SSLCommerzService(
        IOptions<SSLCommerzSettings> options,
        IHttpClientFactory httpClientFactory)
    {
        _settings = options.Value;
        _httpClientFactory = httpClientFactory;

        SSLCommerzValidator.ValidateSettings(_settings);
    }

    public async Task<SSLCommerzInitResponse> CreatePaymentAsync(
        SSLCommerzPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        SSLCommerzValidator.ValidatePaymentRequest(request);

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_settings.BaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.Add("Accept", "application/json");

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
            ["cus_add1"] = request.CustomerAddress,
            ["cus_city"] = request.CustomerCity,
            ["cus_country"] = request.CustomerCountry,
            ["product_name"] = "Online Payment",
            ["product_category"] = "Service",
            ["product_profile"] = "general",
            ["shipping_method"] = "NO"
        };

        using var response = await client.PostAsync(
            "/gwprocess/v4/api.php",
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

    public async Task<SSLCommerzValidationResponse> ValidatePaymentAsync(
        string validationId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(validationId))
            throw new ArgumentException("ValidationId is required.");

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_settings.BaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        var url = $"/validator/api/validationserverAPI.php?val_id={validationId}" +
                  $"&store_id={_settings.StoreId}&store_passwd={_settings.StorePassword}&format=json";

        using var response = await client.GetAsync(url, cancellationToken);
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