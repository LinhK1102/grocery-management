using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObjects.Commons
{
    public class PagedResult<T>
    {
        public int TotalItems { get; set; }
        [JsonPropertyName("items")]
        public List<T> Items { get; set; } = new();
    }

}
