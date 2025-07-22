using Microsoft.AspNetCore.Mvc;
using GroceryWebApp.Models.Dto;
using GroceryWebApp.Services;

namespace GroceryWebApp.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly WarehouseApiService _warehouseApiService;

        public WarehouseController(WarehouseApiService warehouseApiService)
        {
            _warehouseApiService = warehouseApiService;
        }

        public async Task<IActionResult> Index()
        {
            var warehouses = await _warehouseApiService.GetAllAsync();
            return View(warehouses);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseDto warehouse)
        {
            if (!ModelState.IsValid) return View(warehouse);

            var success = await _warehouseApiService.CreateAsync(warehouse);
            if (success) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Unable to create warehouse.");
            return View(warehouse);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var warehouse = await _warehouseApiService.GetByIdAsync(id);
            if (warehouse == null) return View("NotFound");
            return View(warehouse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WarehouseDto warehouse)
        {
            if (id != warehouse.WarehouseId) return NotFound();
            if (!ModelState.IsValid) return View(warehouse);

            var success = await _warehouseApiService.UpdateAsync(id, warehouse);
            if (success) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Unable to update warehouse.");
            return RedirectToAction(nameof(Edit), id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _warehouseApiService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
