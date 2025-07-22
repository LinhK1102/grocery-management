using System.Text.Json.Serialization;

namespace GroceryWebApp.Models.Dto
{
    public class WarehouseDto
    {
        [JsonPropertyName("warehouseId")]
        public int WarehouseId { get; set; }

        [JsonPropertyName("warehouseName")]
        public string WarehouseName { get; set; }

        [JsonPropertyName("warehouseLocation")]
        public string WarehouseLocation { get; set; }
    }

}
