using System.Text.Json.Serialization;

namespace GroceryWebApp.Models.Dto
{
    public class SupplierDto
    {
        [JsonPropertyName("supplierId")]
        public int SupplierId { get; set; }

        [JsonPropertyName("supplierName")]
        public string SupplierName { get; set; }

        [JsonPropertyName("supplierEmail")]
        public string SupplierEmail { get; set; }

        [JsonPropertyName("supplierPhoneNumber")]
        public string SupplierPhoneNumber { get; set; }
    }

}
