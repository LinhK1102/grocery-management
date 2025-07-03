using System.Text.Json.Serialization;

namespace WebApplication.Models.Dto
{
    public class CategoryDto
    {
        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }
    }

}
