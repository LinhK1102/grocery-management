using System.Text.Json.Serialization;

namespace GroceryWebApp.Models
{
    public class ApiListWrapper<T>
    {
        [JsonPropertyName("$values")]
        public List<T>? Values { get; set; }
    }
}
