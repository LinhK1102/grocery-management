using System.Text.Json.Serialization;

namespace BusinessObjects.Commons
{
    public class ApiListWrapper<T>
    {
        [JsonPropertyName("$values")]
        public List<T>? Values { get; set; }
    }
}
