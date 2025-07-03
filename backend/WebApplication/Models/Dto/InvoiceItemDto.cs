using System.Text.Json.Serialization;

namespace WebApplication.Models.Dto
{
    public class InvoiceItemDto
    {
        [JsonPropertyName("invoiceItemId")]
        public int InvoiceItemId { get; set; }

        [JsonPropertyName("invoiceId")]
        public int InvoiceId { get; set; }

        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }
    }

}
