using UniversityEvents.Application.SSLCommerz.Models;

namespace UniversityEvents.Application.SSLCommerz.Validators;

public static class SSLCommerzValidator
{
    public static void ValidateSettings(SSLCommerzSettings settings)
    {
        if (!settings.IsValid())
            throw new InvalidOperationException("Invalid SSLCommerz configuration.");
    }

    public static void ValidatePaymentRequest(SSLCommerzPaymentRequest request)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(request.TransactionId))
            throw new ArgumentException("TransactionId is required.");

        ValidateUrl(request.SuccessUrl, nameof(request.SuccessUrl));
        ValidateUrl(request.FailUrl, nameof(request.FailUrl));
        ValidateUrl(request.CancelUrl, nameof(request.CancelUrl));

        if (string.IsNullOrWhiteSpace(request.CustomerName))
            throw new ArgumentException("CustomerName is required.");

        if (string.IsNullOrWhiteSpace(request.CustomerPhone))
            throw new ArgumentException("CustomerPhone is required.");
    }

    private static void ValidateUrl(string url, string field)
    {
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            throw new ArgumentException($"{field} is not a valid URL.");
    }
}
