using Microsoft.AspNetCore.Mvc;
using GroceryWebApp.Helpers;
using GroceryWebApp.Models.Dto;
using GroceryWebApp.Service;
using GroceryWebApp.Services;
using System.Text.Json;
using Utility.Common;
using System.Security.Claims;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace GroceryWebApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderApiService _orderApiService;
        private readonly OrderDetailApiService _orderDetailApiService;
        private readonly InvoiceApiService _invoiceApiService;
        private readonly ProductApiService _productApiService;
        private readonly PaymentService _paymentService;
        private readonly CustomerApiService _customerApiService;
        private readonly EmployeeApiService _employeeApiService;

        public OrderController(PaymentService paymentService, OrderApiService orderApiService,
            OrderDetailApiService orderDetailApiService, InvoiceApiService invoiceApiService,
            ProductApiService productApiService, CustomerApiService customerApiService, EmployeeApiService employeeApiService)
        {
            _paymentService = paymentService;
            _orderApiService = orderApiService;
            _orderDetailApiService = orderDetailApiService;
            _invoiceApiService = invoiceApiService;
            _productApiService = productApiService;
            _customerApiService = customerApiService;
            _employeeApiService = employeeApiService;
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
            var dto = await _productApiService.GetAllAsync();
            var products = dto.Items;
            ViewBag.Products = products.OrderBy(p => p.ProductName).ToList() ?? new List<ProductDto>();

            return View(new OrderOrInvoiceDto
            {
                Items = new List<OrderItemDto> { new() } // default item
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderOrInvoiceDto dto)
        {
            foreach (var key in Request.Form.Keys)
            {
                Console.WriteLine($"{key}: {Request.Form[key]}");
            }


            if (!ModelState.IsValid)
            {
                var productDto = await _productApiService.GetAllAsync();
                var products = dto.Items;
                ViewBag.Products = products.OrderBy(p => p.ProductName).ToList();
                return View(dto);
            }

            if (dto.Items == null || !dto.Items.Any())
            {
                var dtos = await _productApiService.GetAllAsync();
                var products = dtos.Items;
                ViewBag.Products = products.OrderBy(p => p.ProductName).ToList() ?? new List<ProductDto>();

                ModelState.AddModelError("", "No order items selected.");
                return View(dto);
            }


            dto.CustomerName = (string.IsNullOrWhiteSpace(dto.CustomerName)
                || dto.CustomerName.ToLower().Contains(UtitlityConstant.Customer_Default_Name.ToLower()))
                                ? UtitlityConstant.Customer_Default_Name : dto.CustomerName;
            if (dto.IsBanking)
            {
                // Create PayOS QR & redirect
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var total = (double)dto.Items.Sum(i => i.UnitPrice * i.Quantity);
                var request = new CreateRequest(total, $"{dto.CustomerName}", baseUrl);
                var res = await _paymentService.CreateAsync(request);

                if (res?.Data?.CheckoutUrl != null)
                    return Redirect(res.Data.CheckoutUrl);

                TempData["Error"] = $"Failed to initiate payment link due to [{res.Desc}]";
                TempData["QrCode"] = res?.Data?.QrCode;

                var productDto = await _productApiService.GetAllAsync();
                var products = productDto.Items;
                //var products = await _productApiService.GetAllAsync();
                ViewBag.Products = products.OrderBy(p => p.ProductName).ToList() ?? new List<ProductDto>();

                return View("Create", dto);
            }

            var email = User.FindFirst(ClaimTypes.Name)?.Value;
            var employeeResult = await _employeeApiService.SearchByEmailName(email);

            if (employeeResult == null)
            {
                return NotFound("Employee not found.");
            }

            var employeeId = employeeResult.EmployeeId;

            var customerId = (await _customerApiService.SearchCustomersAsync(dto.CustomerName)).FirstOrDefault().CustomerId;

            var orderDto = new OrderUpdateDto
            {
                CustomerId = customerId,
                EmployeeId = employeeId,
                OrderDate = dto.OrderDate,
                Items = dto.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            var b = await _orderApiService.CreateAsync(orderDto);
            if (b)
                return RedirectToAction(nameof(PaymentSuccess));

            return RedirectToAction(nameof(PaymentCancel));
        }

        [HttpGet]
        public IActionResult PaymentSuccess()
        {
            TempData["Success"] = "✅ Payment completed successfully!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult PaymentCancel()
        {
            TempData["Error"] = "❌ Payment was cancelled due to update fail";
            return RedirectToAction("Create", "Order");
        }


    }
}
