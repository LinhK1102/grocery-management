using System.Text.Json.Serialization;

namespace GroceryWebApp.Models.Dto
{
    public class ProductWarehouseDto
    {
        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [JsonPropertyName("warehouseId")]
        public int WarehouseId { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }

}
