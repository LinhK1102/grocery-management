using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Entities
{
    public class Item
    {
        public string ItemId { get; set; }
        public string BatchCode { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string Barcode { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime ImportedDate { get; set; }
        public DateTime ManufactureDate { get; set; }

        public int Quantity { get; set; }
    }

}
