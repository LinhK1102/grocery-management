using System.Text.Json.Serialization;

namespace GroceryWebApp.Models.Dto
{
    public class OrderDto
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
        [JsonIgnore]
        public List<OrderItemDto> Items { get; set; } = new();
    }

        public class OrderDetailViewDto
        {
            public int OrderDetailId { get; set; }
            public int OrderId { get; set; }
            public int ProductId { get; set; }

            public string? ProductName { get; set; } // Thêm tên hiển thị
            public int Quantity { get; set; }
            public decimal UnitPriceAtTimeOfSale { get; set; }
            public decimal DiscountApplied { get; set; }
        }
    public class OrderDetailsViewModel
    {
        public OrderDto Order { get; set; } = default!;
        public List<OrderDetailViewDto> Details { get; set; } = new();

        public string CustomerName { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string OutletName { get; set; } = "";
        public string WarehouseName { get; set; } = "";
    }


}
