using System.Text.Json.Serialization;

namespace WebApplication.Models.Dto
{
    public class OrderDetailDto
    {
        [JsonPropertyName("orderDetailId")]
        public int OrderDetailId { get; set; }

        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }

        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("unitPriceAtTimeOfSale")]
        public decimal UnitPriceAtTimeOfSale { get; set; }

        [JsonPropertyName("discountApplied")]
        public decimal DiscountApplied { get; set; }
    }

}
