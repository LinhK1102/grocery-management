using Microsoft.AspNetCore.Mvc;
using WebApplication.Models.Dto;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeApiService _employeeApiService;

        public EmployeeController(EmployeeApiService employeeApiService)
        {
            _employeeApiService = employeeApiService;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeApiService.GetAllAsync();
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
        public async Task<IActionResult> Create(EmployeeDto employee)
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
            if (employee == null) return View("NotFound");
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeDto employee)
        {
            if (id != employee.EmployeeId) return NotFound();
            if (!ModelState.IsValid) return View(employee);

            var success = await _employeeApiService.UpdateAsync(id, employee);
            if (success) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Unable to update employee.");
            return View(employee);
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
    }
}
