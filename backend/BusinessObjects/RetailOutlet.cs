using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class RetailOutlet
    {
        public int RetailOutletId { get; set; }
        public string RetailOutletName { get; set; }
        public string RetailOutletLocation { get; set; }

        [JsonIgnore]
        public ICollection<Order> Orders { get; set; }
    }

}
