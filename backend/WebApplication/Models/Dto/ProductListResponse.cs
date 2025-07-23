using System.Text.Json.Serialization;

namespace GroceryWebApp.Models.Dto
{
    public class ProductListResponse
    {
        public int Total { get; set; }

        [JsonPropertyName("products")]
        public ProductListWrapper Products { get; set; } = new();

        public class ProductListWrapper
        {
            [JsonPropertyName("$values")]
            public List<ProductDto> Values { get; set; } = new();
        }
        public class OrderDetailWrapper
        {
            [JsonPropertyName("$values")]
            public List<OrderDetailDto> Values { get; set; }
        }

    }

}
