using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public int? OutletId { get; set; }
        public int? WarehouseId { get; set; }

        [JsonIgnore] 
        public Customer Customer { get; set; }
        [JsonIgnore] 
        public Employee Employee { get; set; }
        [JsonIgnore] 
        public RetailOutlet RetailOutlet { get; set; }
        [JsonIgnore]
        public Warehouse Warehouse { get; set; }
        [JsonIgnore]
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }

}
