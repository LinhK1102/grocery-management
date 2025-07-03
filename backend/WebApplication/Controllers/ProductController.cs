using Microsoft.AspNetCore.Mvc;
using WebApplication.Services;
using WebApplication.Models.Dto;

namespace WebApplication.Controllers;
public class ProductController : Controller
{
    private readonly ProductApiService _productApiService;

    public ProductController(ProductApiService productApiService)
    {
        _productApiService = productApiService;
    }

    public async Task<IActionResult> Index(string search = "", int page = 1, int pageSize = 10)
    {
        var allProducts = await _productApiService.GetAllAsync();

        if (!string.IsNullOrEmpty(search))
        {
            allProducts = allProducts
                .Where(p => p.ProductName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var total = allProducts.Count;
        var items = allProducts.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.CurrentSearch = search;
        ViewBag.Page = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productApiService.GetByIdAsync(id);
        if (product == null) return NotFound(); // <-- render view
        return View(product);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        var result = await _productApiService.CreateAsync(dto);
        if (result)
            return RedirectToAction("Index");
        ModelState.AddModelError("", "Failed to create product.");
        return View(dto);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productApiService.GetByIdAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, ProductDto dto)
    {
        if (id != dto.ProductId) return BadRequest();
        if (!ModelState.IsValid) return View(dto);
        var result = await _productApiService.UpdateAsync(id, dto);
        if (result)
            return RedirectToAction("Index");
        ModelState.AddModelError("", "Failed to update product.");
        return View(dto);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productApiService.GetByIdAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _productApiService.DeleteAsync(id);
        return RedirectToAction("Index");
    }
}
