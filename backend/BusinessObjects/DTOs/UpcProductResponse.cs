using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs
{
    public class UpcProductResponse
    {
        public string? Barcode { get; set; }
        public string? Title { get; set; }
        public string? Alias { get; set; }
        public string? Description { get; set; }
        public string? Brand { get; set; }
        public string? Manufacturer { get; set; }
        public string? Mpn { get; set; }
        public string? Msrp { get; set; }
        public string? ASIN { get; set; }
        public string? Category { get; set; }
        public List<string>? Images { get; set; }
        public List<StoreInfo>? Stores { get; set; }
        public ReviewInfo? Reviews { get; set; }
        public bool Status { get; set; }
    }

    public class StoreInfo
    {
        public string? Store { get; set; }
        public string? Price { get; set; }
        public string? Url { get; set; }
    }

    public class ReviewInfo
    {
        public int Thumbsup { get; set; }
        public int Thumbsdown { get; set; }
    }
}
