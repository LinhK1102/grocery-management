using System.Text.Json.Serialization;

namespace BusinessObjects.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public int UnitsInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public string BarcodeValue { get; set; }

        public DateTime ExpiryDate { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ImportedDate { get; set; }

        public ICollection<Item> Items { get; set; }

        [JsonIgnore]
        public Category Category { get; set; }
        [JsonIgnore]
        public Supplier Supplier { get; set; }

        [JsonIgnore]
        public ICollection<OrderDetail> OrderDetails { get; set; }

        [JsonIgnore]
        public ICollection<ProductWarehouse> ProductWarehouses { get; set; }
    }


}
