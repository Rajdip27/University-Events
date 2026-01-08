using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.SSLCommerz.Models;
using UniversityEvents.Core.Entities;

namespace UniversityEvents.Application.Helpers;

public static class PaymentHelper
{
    public static Payment UpdateFromSslResponse(SSLCommerzValidationResponse response)
    {
        Payment payment= new Payment();
        payment.InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{new Random().Next(1000, 9999)}";
        payment.Provider = "SSLCommerz";
        payment.ProviderSessionId = response.BankTranId;
        payment.Status = response.Status;
        payment.TransactionId = response.TranId;
        payment.ValidationId = response.ValId;
        payment.Amount = NullableExtensions.ParseAmount(response.Amount);

        return payment;
    }
}

