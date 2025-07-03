using System.Text.Json.Serialization;

namespace WebApplication.Models.Dto
{
    public class ItemDto
    {
        [JsonPropertyName("itemId")]
        public string ItemId { get; set; }

        [JsonPropertyName("batchCode")]
        public string BatchCode { get; set; }

        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [JsonPropertyName("barcode")]
        public string Barcode { get; set; }

        [JsonPropertyName("expiryDate")]
        public DateTime ExpiryDate { get; set; }

        [JsonPropertyName("importedDate")]
        public DateTime ImportedDate { get; set; }

        [JsonPropertyName("manufactureDate")]
        public DateTime ManufactureDate { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [JsonIgnore]
        public ProductDto Product { get; set; }

    }

}
