using System.Text.Json.Serialization;

namespace GroceryWebApp.Models.Dto
{
    public class RetailOutletDto
    {
        [JsonPropertyName("retailOutletId")]
        public int RetailOutletId { get; set; }

        [JsonPropertyName("retailOutletName")]
        public string RetailOutletName { get; set; }

        [JsonPropertyName("retailOutletLocation")]
        public string RetailOutletLocation { get; set; }
    }

}
