using System.Text.Json.Serialization;

namespace GroceryWebApp.Models.Dto
{
    public class OrderUpdateDto
    {
        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }

        [JsonPropertyName("orderDate")]
        public DateTime OrderDate { get; set; }

        [JsonPropertyName("customerId")]
        public int CustomerId { get; set; }

        [JsonPropertyName("employeeId")]
        public int EmployeeId { get; set; }

        [JsonPropertyName("outletId")]
        public int? OutletId { get; set; }

        [JsonPropertyName("warehouseId")]
        public int? WarehouseId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
