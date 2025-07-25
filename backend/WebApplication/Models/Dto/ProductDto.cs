using BusinessObjects.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Utility.Common;

namespace GroceryWebApp.Models.Dto
{
    public class ProductDto
    {
        [Key]
        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        [JsonPropertyName("categoryId")]
        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        [JsonPropertyName("supplierId")]
        [Required(ErrorMessage = "Supplier is required.")]
        public int SupplierId { get; set; }

        [JsonPropertyName("unitsInStock")]
        public int UnitsInStock { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("barcodeValue")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Barcode must contain digits only.")]
        public string BarcodeValue { get; set; }

        [JsonPropertyName("expiryDuration")]
        public DateTime ExpiryDuration { get; set; }

        [JsonPropertyName("items")]
        [JsonIgnore]
        [JsonConverter(typeof(ValuesWrapperConverter<ItemDto>))]
        public ICollection<ItemDto> Items { get; set; } = new List<ItemDto>();

        [JsonPropertyName("orderDetails")]
        [JsonIgnore]
        [JsonConverter(typeof(ValuesWrapperConverter<OrderDetailDto>))]
        public ICollection<OrderDetailDto> OrderDetails { get; set; } = new List<OrderDetailDto>();

        [JsonIgnore]
        public CategoryDto Category { get; set; }

        [JsonIgnore]    
        public SupplierDto Supplier { get; set; }
        [JsonIgnore]
        public OrderDetailDto OrderDetail { get; set; }

        [JsonIgnore]
        public ICollection<ProductWarehouseDto> ProductWarehouses { get; set; } = new List<ProductWarehouseDto>();
    }

    public class CreateProductRequestDto
    {
        [JsonPropertyName("productName")]
        [Required(ErrorMessage = "Product name is required.")]
        public string ProductName { get; set; }

        [JsonPropertyName("categoryId")]
        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        [JsonPropertyName("supplierId")]
        [Required(ErrorMessage = "Supplier is required.")]
        public int SupplierId { get; set; }

        [JsonPropertyName("unitsInStock")]
        public int UnitsInStock { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("barcodeValue")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Barcode must contain digits only.")]
        public string BarcodeValue { get; set; }

        [JsonPropertyName("expiryDuration")]
        public DateTime ExpiryDuration { get; set; }
    }
}
