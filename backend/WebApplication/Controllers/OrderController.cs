using Microsoft.AspNetCore.Mvc;
using WebApplication.Helpers;
using WebApplication.Models.Dto;
using WebApplication.Service;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderApiService _orderApiService;
        private readonly OrderDetailApiService _orderDetailApiService;
        private readonly InvoiceApiService _invoiceApiService;
        private readonly ProductApiService _productApiService;

        public OrderController(OrderApiService orderApiService, OrderDetailApiService orderDetailApiService, InvoiceApiService invoiceApiService, ProductApiService productApiService)
        {
            _orderApiService = orderApiService;
            _orderDetailApiService = orderDetailApiService;
            _invoiceApiService = invoiceApiService;
            _productApiService = productApiService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderApiService.GetAllAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderApiService.GetByIdAsync(id);
            if (order == null) return View("NotFound");

            var details = await _orderDetailApiService.GetByOrderIdAsync(id);
            return View(Tuple.Create(order, details));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var products = await _productApiService.GetAllAsync();
            ViewBag.Products = products.OrderBy(p => p.ProductName).ToList()
                                ?? new List<ProductDto>(); ;

            return View(new OrderOrInvoiceDto
            {
                Items = new List<OrderItemDto> { new() } // item mặc định
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderOrInvoiceDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            dto.CustomerName = string.IsNullOrWhiteSpace(dto.CustomerName) ? "Unknown" : dto.CustomerName;

            if (dto.IsPaid)
            {
                // Gọi service tạo hóa đơn (Invoice)
                //await _invoiceApiService.CreateAsync(dto); // giả định bạn xử lý được
            }
            else
            {
                // Gọi service tạo đơn hàng (Order)
                //await _orderApiService.CreateAsync(dto);
            }

            //return RedirectToAction("Index", "Order");
            return RedirectToAction("Index", "Home");
        }

    }
}
