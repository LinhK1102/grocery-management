using Microsoft.AspNetCore.Mvc;
using WebApplication.Services;
using WebApplication.Models.Dto;
using WebApplication.Service;

namespace WebApplication.Controllers;
public class CategoryController : Controller
{
    private readonly CategoryApiService _categoryApiService;

    public CategoryController(CategoryApiService categoryApiService)
    {
        _categoryApiService = categoryApiService;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryApiService.GetAllAsync();
        return View(categories);
    }

    public async Task<IActionResult> Details(int id)
    {
        var category = await _categoryApiService.GetByIdAsync(id);
        if (category == null) return View("NotFound");
        return View(category);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryDto category)
    {
        if (!ModelState.IsValid) return View(category);

        var success = await _categoryApiService.CreateAsync(category);
        if (success) return RedirectToAction(nameof(Index));

        ModelState.AddModelError("", "Unable to create category.");
        return View(category);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryApiService.GetByIdAsync(id);
        if (category == null) return View();
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryDto category)
    {
        if (id != category.CategoryId) return NotFound();
        if (!ModelState.IsValid) return View(category);

        var success = await _categoryApiService.UpdateAsync(id, category);
        if (success) return RedirectToAction(nameof(Index));

        ModelState.AddModelError("", "Unable to update category.");
        return RedirectToAction("Edit", id);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categoryApiService.GetByIdAsync(id);
        if (category == null) return View("NotFound");
        return View(category);
    }

}
