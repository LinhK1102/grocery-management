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

        public OrderController(OrderApiService orderApiService, OrderDetailApiService orderDetailApiService, InvoiceApiService invoiceApiService)
        {
            _orderApiService = orderApiService;
            _orderDetailApiService = orderDetailApiService;
            _invoiceApiService = invoiceApiService;
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
            var model = new OrderOrInvoiceDto
            {
                Items = new List<OrderItemDto> { new() }  // default 1 dòng
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderOrInvoiceDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            if (dto.IsPaid)
            {
                // Logic xử lý tạo Invoice
                 //await _invoiceApiService.Create(dto);
                Console.WriteLine("Creating invoice...");
            }
            else
            {
                // Logic xử lý tạo Order
                // Ví dụ giả định bạn có customerId, employeeId...
                int customerId = dto.CustomerName == null 
                    ? 1 //await _customerService.GetCustomerIdByNameAsync(dto.CustomerName) 
                    :0;
                //int employeeId = ControllerExtensions.GetEmployeeIdFromSession(); // hoặc 1 cách nào đó

                var converted = DtoExtensions.ConvertToOrderDto(dto, customerId, 1);
                await _orderApiService.CreateAsync(converted);
                Console.WriteLine("Creating order...");
            }

            return RedirectToAction("Index");
        }
    }
}
