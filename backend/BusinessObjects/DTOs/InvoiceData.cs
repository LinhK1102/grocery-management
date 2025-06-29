using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs
{
    public class InvoiceData
    {
        public string InvoiceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CustomerName { get; set; }
        public List<InvoiceItem> Items { get; set; }
        public decimal Total => Items.Sum(i => i.Quantity*i.UnitPrice);
    }
}
