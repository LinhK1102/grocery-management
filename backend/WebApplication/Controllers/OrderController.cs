using Microsoft.AspNetCore.Mvc;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderApiService _orderApiService;
        private readonly OrderDetailApiService _orderDetailApiService;

        public OrderController(OrderApiService orderApiService, OrderDetailApiService orderDetailApiService)
        {
            _orderApiService = orderApiService;
            _orderDetailApiService = orderDetailApiService;
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
    }
}
