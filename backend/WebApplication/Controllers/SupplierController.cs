using Microsoft.AspNetCore.Mvc;
using WebApplication.Models.Dto;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class SupplierController : Controller
    {
        private readonly SupplierApiService _supplierApiService;

        public SupplierController(SupplierApiService supplierApiService)
        {
            _supplierApiService = supplierApiService;
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
            if (id != supplier.SupplierId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(supplier);

            var success = await _supplierApiService.UpdateAsync(id, supplier);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Unable to update supplier.");
            return View(supplier);
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
