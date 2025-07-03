using System.Text.Json.Serialization;

namespace WebApplication.Models
{
    public class ApiListWrapper<T>
    {
        [JsonPropertyName("$values")]
        public List<T>? Values { get; set; }
    }
}
