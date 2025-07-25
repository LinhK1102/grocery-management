using System.Text.Json.Serialization;

namespace GroceryWebApp.Models.Dto
{
    public class CreateResponse
    {
        public CreateResponseData data { get; set; }
        public string signature { get; set; }
    }
    public class CreateResponseData
    {
        public string bin { get; set; }
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public double amount { get; set; }
        public string description { get; set; }
        public int orderCode { get; set; }
        public string currency { get; set; }
        public string paymentLinkId { get; set; }
        public string status { get; set; }
        public string checkoutUrl { get; set; }
        public string qrCode { get; set; }
    }
    public class CreateRequest
    {
        public double amount { get; set; }
        public string description { get; set; }
        public string returnUrl { get; set; }
        public string cancelUrl { get; set; }

        public CreateRequest(double amount, string description, string baseUrl)
        {
            this.amount = amount;
            this.description = description;
            this.returnUrl = $"{baseUrl}/Order/Success";
            this.cancelUrl = $"{baseUrl}/Order/Cancel";
        }
    }

    public class PaymentDto
    {
    }

    public class PayOSResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("desc")]
        public string Desc { get; set; }

        [JsonPropertyName("data")]
        public PayOSData Data { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }
    }

    public class PayOSData
    {
        [JsonPropertyName("bin")] public string Bin { get; set; }
        [JsonPropertyName("accountNumber")] public string AccountNumber { get; set; }
        [JsonPropertyName("accountName")] public string AccountName { get; set; }
        [JsonPropertyName("amount")] public int Amount { get; set; }
        [JsonPropertyName("description")] public string Description { get; set; }
        [JsonPropertyName("orderCode")] public long OrderCode { get; set; }
        [JsonPropertyName("currency")] public string Currency { get; set; }
        [JsonPropertyName("paymentLinkId")] public string PaymentLinkId { get; set; }
        [JsonPropertyName("status")] public string Status { get; set; }
        [JsonPropertyName("checkoutUrl")] public string CheckoutUrl { get; set; }
        [JsonPropertyName("qrCode")] public string QrCode { get; set; }
    }



}
