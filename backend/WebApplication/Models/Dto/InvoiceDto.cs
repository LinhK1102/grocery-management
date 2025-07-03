using System.Text.Json.Serialization;

namespace WebApplication.Models.Dto
{
    public class InvoiceDto
    {
        [JsonPropertyName("invoiceId")]
        public int InvoiceId { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; }

        [JsonPropertyName("invoiceFilePath")]
        public string InvoiceFilePath { get; set; }
    }

}
