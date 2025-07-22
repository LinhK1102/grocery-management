using System.ComponentModel.DataAnnotations;

namespace GroceryWebApp.Models.Dto
{
    public class ProductUpdateDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int CategoryId { get; set; }

        public int SupplierId { get; set; }

        public int UnitsInStock { get; set; }

        public decimal UnitPrice { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Barcode must contain digits only.")]
        public string BarcodeValue { get; set; }

        public DateTime ExpiryDuration { get; set; }
    }
}
