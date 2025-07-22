using System.Text.Json.Serialization;

namespace GroceryWebApp.Models.Dto
{
    public class CustomerDto
    {
        [JsonPropertyName("customerId")]
        public int CustomerId { get; set; }

        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; }

        [JsonPropertyName("customerType")]
        public string CustomerType { get; set; }

        [JsonPropertyName("discountRate")]
        public decimal DiscountRate { get; set; }
    }

}
