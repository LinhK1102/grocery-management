using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GroceryWebApp.Models.Dto
{
    public class ItemCreateDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be greater than 0.")]
        public int ProductId { get; set; }
        [Required]
        public string BatchCode { get; set; }
        public string Barcode { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime ImportedDate { get; set; }
        public DateTime ManufactureDate { get; set; }
        public int Quantity { get; set; }
    }

}
