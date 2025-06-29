using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Entities
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceFilePath { get; set; }
        public ICollection<InvoiceItem> InvoiceItems { get; set; }
    }
}
