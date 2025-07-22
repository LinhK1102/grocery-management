using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repositories.DTOs;
using GroceryWebApp.Models.Dto;
using GroceryWebApp.Services;

namespace GroceryWebApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeApiService _employeeApiService;
        private readonly RetailOutletApiService _retailOutletApiService;

        public EmployeeController(EmployeeApiService employeeApiService, RetailOutletApiService retailOutletApiService)
        {
            _employeeApiService = employeeApiService;
            _retailOutletApiService = retailOutletApiService;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeApiService.GetAllAsync();

            var outlets = await _retailOutletApiService.GetAllAsync();
            // RetailOutletId -> RetailOutletName
            ViewBag.OutletMap = outlets.ToDictionary(o => o.RetailOutletId, o => o.RetailOutletName);

            return View(employees);
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeApiService.GetByIdAsync(id);
            if (employee == null) return View("NotFound");
            return View(employee);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeRegisterRequest employee)
        {
            if (!ModelState.IsValid) return View(employee);

            var success = await _employeeApiService.CreateAsync(employee);
            if (success) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Unable to create employee.");
            return View(employee);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeApiService.GetByIdAsync(id);

            await SetRetailOutletDropdown();

            if (employee == null) return View("NotFound");
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeDto employee)
        {
            if (id != employee.EmployeeId) return NotFound();

            if (!ModelState.IsValid)
            {
                await SetRetailOutletDropdown();
                return View(employee);
            }

            var success = await _employeeApiService.UpdateAsync(id, employee);
            if (success) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Unable to update employee.");
            await SetRetailOutletDropdown();
            return View(employee);
        }

        private async Task SetRetailOutletDropdown()
        {
            ViewBag.RetailOutletList = (await _retailOutletApiService.GetAllAsync())
                .Select(ro => new SelectListItem
                {
                    Value = ro.RetailOutletId.ToString(),
                    Text = ro.RetailOutletName
                }).ToList();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeApiService.GetByIdAsync(id);
            if (employee == null) return View("NotFound");
            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _employeeApiService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        //[HttpGet("search")]
        //public IActionResult SearchByName([FromQuery] string name)
        //{
        //    var employee = await _employeeApiService.Sear
        //    if (employee == null)
        //        return NotFound();

        //    return Ok(new
        //    {
        //        employee.EmployeeId,
        //        employee.FullName
        //    });
        //}

    }
}
