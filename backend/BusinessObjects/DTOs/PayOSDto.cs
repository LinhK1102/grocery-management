using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs
{
    public class CreatePaymentRequest
    {
        public int amount { get; set; }
        public string? cancelUrl { get; set; }
        public string? description { get; set; }
        public string? returnUrl { get; set; }
    }
    internal class PayOSDto
    {
    }
}
