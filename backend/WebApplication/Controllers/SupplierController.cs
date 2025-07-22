using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Utility.Hubs;
using GroceryWebApp.Helpers;
using GroceryWebApp.Models.Dto;
using GroceryWebApp.Services;

namespace GroceryWebApp.Controllers
{
    public class SupplierController : Controller
    {
        private readonly SupplierApiService _supplierApiService;
        private readonly IHubContext<NotificationHub> _hubContext;
        
        public SupplierController(SupplierApiService supplierApiService, IHubContext<NotificationHub> hubContext)
        {
            _supplierApiService = supplierApiService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierApiService.GetAllAsync();
            return View(suppliers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _supplierApiService.GetByIdAsync(id);
            if (supplier == null)
                return View("NotFound");

            return View(supplier);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierDto supplier)
        {
            if (!ModelState.IsValid)
                return View(supplier);

            var success = await _supplierApiService.CreateAsync(supplier);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Unable to create supplier.");
            return View(supplier);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _supplierApiService.GetByIdAsync(id);
            if (supplier == null)
                return View("NotFound");

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupplierDto supplier)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _supplierApiService.UpdateAsync(id, supplier);

            Response.Cookies.Append("Status", "Fail", new CookieOptions { Expires = DateTimeOffset.UtcNow.AddSeconds(5) });
            Response.Cookies.Append("Message", "Cập nhật thất bại!", new CookieOptions { Expires = DateTimeOffset.UtcNow.AddSeconds(5) });

            if (success)
                RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(Edit), new { id = supplier.SupplierId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _supplierApiService.GetByIdAsync(id);
            if (supplier == null)
                return View("NotFound");

            return View(supplier);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _supplierApiService.DeleteAsync(id);
            
            return RedirectToAction(nameof(Index));
        }
    }
}
