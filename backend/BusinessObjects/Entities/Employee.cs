using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObjects.Entities
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public string EmployeeEmailTokenPass { get; set; }
        [JsonIgnore]
        public ICollection<Order> Orders { get; set; }

        public int RetailOutletId { get; set; }
        [JsonIgnore]
        public RetailOutlet RetailOutlet { get; set; }
    }

}
