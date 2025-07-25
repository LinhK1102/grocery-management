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
        private readonly RetailOutletApiService _retailOutletApiService;
        private readonly WarehouseApiService _warehouseApiService;

        public OrderController(PaymentService paymentService, OrderApiService orderApiService,
            OrderDetailApiService orderDetailApiService, InvoiceApiService invoiceApiService,
            ProductApiService productApiService, CustomerApiService customerApiService, EmployeeApiService employeeApiService, 
            RetailOutletApiService retailOutletApiService, WarehouseApiService warehouseApiService)
        {
            _paymentService = paymentService;
            _orderApiService = orderApiService;
            _orderDetailApiService = orderDetailApiService;
            _invoiceApiService = invoiceApiService;
            _productApiService = productApiService;
            _customerApiService = customerApiService;
            _employeeApiService = employeeApiService;
            _retailOutletApiService = retailOutletApiService;
            _warehouseApiService = warehouseApiService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderApiService.GetAllAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderApiService.GetByIdAsync(id);
            var details = await _orderDetailApiService.GetByOrderIdAsync(id);

            var customer = (await _customerApiService.SearchCustomersAsync(UtitlityConstant.Customer_Type_Unknown)).FirstOrDefault();
            var employee = await _employeeApiService.GetByIdAsync(order.EmployeeId);
            var outlet = order.OutletId != null ? await _retailOutletApiService.GetByIdAsync(order.OutletId.Value) : null;
            var warehouse = order.WarehouseId != null ? await _warehouseApiService.GetByIdAsync(order.WarehouseId.Value) : null;

            var detailViewDtos = new List<OrderDetailViewDto>();
            foreach (var item in details)
            {
                var product = await _productApiService.GetByIdAsync(item.ProductId);
                detailViewDtos.Add(new OrderDetailViewDto
                {
                    OrderDetailId = item.OrderDetailId,
                    OrderId = item.OrderId,
                    ProductId = item.ProductId,
                    ProductName = product.ProductName,
                    Quantity = item.Quantity,
                    UnitPriceAtTimeOfSale = item.UnitPriceAtTimeOfSale,
                    DiscountApplied = item.DiscountApplied
                });
            }

            var vm = new OrderDetailsViewModel
            {
                Order = order,
                Details = detailViewDtos,
                CustomerName = customer?.CustomerName   ?? "N/A",
                EmployeeName = employee?.EmployeeName ?? "N/A",
                OutletName = outlet?.RetailOutletName ?? "N/A",
                WarehouseName = warehouse?.WarehouseName ?? "N/A"
            };

            return View(vm);
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
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            var order = await _orderApiService.CreateAsync(orderDto);
            if (dto.IsBanking && order != null)
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
            else if (order != null)
                return RedirectToAction(nameof(PaymentSuccess), new { orderId = order.OrderId });

            return RedirectToAction(nameof(PaymentCancel));
        }

        [HttpGet]
        public IActionResult PaymentSuccess(int orderId)
        {
            TempData["Success"] = "✅ Payment completed successfully!";
            return RedirectToAction(nameof(Details),new {id = orderId});
        }

        [HttpGet]
        public IActionResult PaymentCancel()
        {
            TempData["Error"] = "❌ Payment was cancelled due to update fail";
            return RedirectToAction("Create", "Order");
        }


    }
}
