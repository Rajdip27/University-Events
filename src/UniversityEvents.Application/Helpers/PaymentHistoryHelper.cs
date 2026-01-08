using System.Text.Json;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.SSLCommerz.Models;
using UniversityEvents.Core.Entities;

namespace UniversityEvents.Application.Helpers;

public static class PaymentHistoryHelper
{
    public static PaymentHistory CreateFromSslResponse(
    SSLCommerzValidationResponse response)
    {
        return new PaymentHistory
        {
            Provider = "SSLCommerz",
            ProviderSessionId = response.BankTranId,
            Status = response.Status,
            TransactionId = response.TranId,
            ValidationId = response.ValId,
            BankName = response.CardIssuer,
            CustomerName = response.CustomerName,
            CustomerMobile = response.CustomerMobile,
            Amount = NullableExtensions.ParseAmount(response.Amount),
            Currency = response.Currency,
            JsonResponse = System.Text.Json.JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                WriteIndented = true
            })
        };
    }
}
