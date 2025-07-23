using Microsoft.AspNetCore.Mvc;
using GroceryWebApp.Services;
using GroceryWebApp.Models.Dto;
using GroceryWebApp.Service;
using System.Text.Json;
using GroceryWebApp.Helpers;
using static GroceryWebApp.Constants.SystemMessages;
using Utility.Common;
using static GroceryWebApp.Constants.ApiRoutes;
using System.Drawing.Printing;

namespace GroceryWebApp.Controllers;
public class ProductController : Controller
{
    private readonly ProductApiService _productApiService;
    private readonly CategoryApiService _categoryApiService;
    private readonly SupplierApiService _supplierApiService;

    public ProductController(ProductApiService productApiService, CategoryApiService categoryApiService, SupplierApiService supplierApiService)
    {
        _productApiService = productApiService;
        _categoryApiService = categoryApiService;
        _supplierApiService = supplierApiService;
    }

    public async Task<IActionResult> Index(
            string productName = "", string barcode = "", int? categoryId = null,
            string selectedPriceRange = "", string selectedStockRange = "",
            int page = 1, int pageSize = 10)
    {
        (int? minPrice, int? maxPrice) = ParseRange(selectedPriceRange);
        (int? minStock, int? maxStock) = ParseRange(selectedStockRange);

        List<ProductDto> products;
        int total;

        if (string.IsNullOrWhiteSpace(productName) &&  string.IsNullOrWhiteSpace(barcode) && categoryId == null &&
            string.IsNullOrEmpty(selectedPriceRange) && string.IsNullOrEmpty(selectedStockRange))
        {
            // Không có search gì cả → gọi GetAllAsync
            products = await _productApiService.GetAllAsync("", page, pageSize);
            total = products.Count(); // Hoặc nếu API trả về total thì bạn dùng luôn
            ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
        }
        else
        {
            (products, total) = await _productApiService.GetODataFilteredAsync(
               productName, barcode, categoryId, minPrice, maxPrice, minStock, maxStock, page, pageSize);

            ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
        }
        //check if null
        products ??= new List<ProductDto>();

        await LoadSearchDataToViewBag(productName, barcode, categoryId, selectedPriceRange, selectedStockRange,
                page, pageSize, total);
        return View(products);

    }

    private async Task LoadSearchDataToViewBag(string productName = "",
        string barcode = "", int? categoryId = null, string selectedPriceRange = "",
        string selectedStockRange = "", int page = 1, int pageSize = 10, int total = 0)
    {
        ViewBag.UnitPriceRanges = UtitlityConstant.UnitPriceRanges;
        ViewBag.StockQuantityRanges = UtitlityConstant.StockQuantityRanges;
        ViewBag.ProductName = productName;
        ViewBag.Barcode = barcode;
        ViewBag.CategoryId = categoryId;
        ViewBag.SelectedPriceRange = selectedPriceRange;
        ViewBag.SelectedStockRange = selectedStockRange;
        ViewBag.Page = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

        var categories = await _categoryApiService.GetAllAsync();
        ViewBag.Categories = categories.Select(c => new CategoryDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName
        }).ToList();

    }
    private bool IsEmptySearch(string name, string barcode, int? cat, int? minP, int? maxP, int? minS, int? maxS)
    {
        return string.IsNullOrWhiteSpace(name)
            && string.IsNullOrWhiteSpace(barcode)
            && cat == null
            && minP == null && maxP == null
            && minS == null && maxS == null;
    }


    private (int? min, int? max) ParseRange(string range)
    {
        if (string.IsNullOrWhiteSpace(range)) return (null, null);

        if (range.EndsWith("-"))
        {
            if (int.TryParse(range.TrimEnd('-'), out int minOnly))
                return (minOnly, null);
        }

        var parts = range.Split("-");
        if (parts.Length == 2 &&
            int.TryParse(parts[0], out int min) &&
            int.TryParse(parts[1], out int max))
        {
            return (min, max);
        }

        return (null, null);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productApiService.GetByIdAsync(id);
        product.Category = await _categoryApiService.GetByIdAsync(product.CategoryId);
        product.Supplier = await _supplierApiService.GetByIdAsync(product.SupplierId);
        if (product == null) return NotFound(); // <-- render view
        return View(product);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductUpdateDto dto)
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
        if (product == null)
        {
            await LoadDropdownDataToViewBag(product);
            return NotFound();
        }

        await LoadDropdownDataToViewBag(product);
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, ProductDto dto)
    {
        if (id != dto.ProductId)
        {
            await LoadDropdownDataToViewBag(dto);
            return BadRequest();
        }

        ModelState.Remove("Category");
        ModelState.Remove("Supplier");
        ModelState.Remove("Item");
        ModelState.Remove("OrderDetail");
        if (!ModelState.IsValid)
        {
            TempData[StatusConstants.TempDataStatus] = StatusConstants.Error;
            TempData[StatusConstants.TempDataMessage] = "Invalid ";

            await LoadDropdownDataToViewBag(dto);
            return View(dto);
        }

        var result = await _productApiService.UpdateAsync(id, DtoExtensions.ProductToUpdateDto(dto));

        if (!result)
        {
            await LoadDropdownDataToViewBag(dto); // <-- thêm dòng này
        }

        return await this.RedirectWithStatusAsync(
                result,
                "Product updated successfully.",
                "Failed to update product.",
                "Index",
                null,
                dto,
                () => LoadDropdownDataToViewBag() // ✅ có thể thay bằng bất kỳ async method nào
        );
    }

    private async Task LoadDropdownDataToViewBag(ProductDto dto = null)
    {
        var categories = await _categoryApiService.GetAllAsync();
        var suppliers = await _supplierApiService.GetAllAsync();

        ViewBag.CategoryJson = JsonSerializer.Serialize(
            categories.Select(c => new { id = c.CategoryId, text = c.CategoryName })
        );

        ViewBag.SupplierJson = JsonSerializer.Serialize(
            suppliers.Select(s => new { id = s.SupplierId, text = s.SupplierName })
        );

        if (dto != null)
        {
            var cat = await _categoryApiService.GetByIdAsync(dto.CategoryId);
            var sup = await _supplierApiService.GetByIdAsync(dto.SupplierId);

            ViewBag.CategoryName = cat?.CategoryName;
            ViewBag.SupplierName = sup?.SupplierName;
        }
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
