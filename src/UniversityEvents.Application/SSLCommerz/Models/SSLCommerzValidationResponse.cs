using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace UniversityEvents.Application.SSLCommerz.Models;

public class SSLCommerzValidationResponse
{
    [Key]
    public int Id { get; set; }

    // Basic identifiers
    [JsonProperty("val_id")]
    public string ValId { get; set; } = string.Empty;

    [JsonProperty("tran_id")]
    public string TranId { get; set; } = string.Empty;

    [JsonProperty("bank_tran_id")]
    public string BankTranId { get; set; } = string.Empty;

    // Bank info
    [JsonProperty("bank_name")]
    public string BankName { get; set; } = string.Empty;

    // Transaction details
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;           // VALID / FAILED / CANCELLED

    [JsonProperty("tran_date")]
    public string TranDate { get; set; } = string.Empty;

    // Amounts as strings from API
    [JsonProperty("amount")]
    public string Amount { get; set; } = "0";

    [JsonProperty("store_amount")]
    public string StoreAmount { get; set; } = "0";

    [JsonProperty("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonProperty("currency_type")]
    public string CurrencyType { get; set; } = string.Empty;

    [JsonProperty("currency_amount")]
    public string CurrencyAmount { get; set; } = "0";

    [JsonProperty("base_fair")]
    public string BaseFair { get; set; } = "0";

    [JsonProperty("currency_rate")]
    public string CurrencyRate { get; set; } = "0";

    // Card details
    [JsonProperty("card_type")]
    public string CardType { get; set; } = string.Empty;

    [JsonProperty("card_no")]
    public string CardNo { get; set; } = string.Empty;

    [JsonProperty("card_issuer")]
    public string CardIssuer { get; set; } = string.Empty;

    [JsonProperty("card_brand")]
    public string CardBrand { get; set; } = string.Empty;

    [JsonProperty("card_issuer_country")]
    public string CardIssuerCountry { get; set; } = string.Empty;

    [JsonProperty("card_issuer_country_code")]
    public string CardIssuerCountryCode { get; set; } = string.Empty;

    [JsonProperty("card_merchant")]
    public string CardMerchant { get; set; } = string.Empty;

    // Customer info
    [JsonProperty("cus_name")]
    public string CustomerName { get; set; } = string.Empty;

    [JsonProperty("cus_phone")]
    public string CustomerMobile { get; set; } = string.Empty;

    // Risk / verification
    [JsonProperty("risk_level")]
    public string RiskLevel { get; set; } = string.Empty;

    [JsonProperty("risk_title")]
    public string RiskTitle { get; set; } = string.Empty;

    [JsonProperty("verify_key")]
    public string VerifyKey { get; set; } = string.Empty;

    [JsonProperty("verify_sign")]
    public string VerifySign { get; set; } = string.Empty;

    // Store info
    [JsonProperty("store_id")]
    public string StoreId { get; set; } = string.Empty;

    // Optional user-defined fields
    [JsonProperty("value_a")]
    public string ValueA { get; set; } = string.Empty;

    [JsonProperty("value_b")]
    public string ValueB { get; set; } = string.Empty;

    [JsonProperty("value_c")]
    public string ValueC { get; set; } = string.Empty;

    [JsonProperty("value_d")]
    public string ValueD { get; set; } = string.Empty;

    // Helper: Is payment successful
    [JsonIgnore]
    public bool IsPaid =>
        Status.Equals("VALID", StringComparison.OrdinalIgnoreCase) ||
        Status.Equals("VALIDATED", StringComparison.OrdinalIgnoreCase);

    // Helper: Parse decimal amounts safely
    [JsonIgnore]
    public decimal AmountDecimal =>
        decimal.TryParse(Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out var value) ? value : 0;

    [JsonIgnore]
    public decimal StoreAmountDecimal =>
        decimal.TryParse(StoreAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out var value) ? value : 0;

    [JsonIgnore]
    public decimal CurrencyAmountDecimal =>
        decimal.TryParse(CurrencyAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out var value) ? value : 0;
}
