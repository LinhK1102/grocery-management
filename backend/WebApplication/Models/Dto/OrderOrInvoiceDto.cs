namespace WebApplication.Models.Dto
{
    public class OrderOrInvoiceDto
    {
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public List<OrderItemDto> Items { get; set; } = new();
        public bool IsPaid { get; set; }  // true => Invoice, false => Order
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }  // optional, for display
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

}
