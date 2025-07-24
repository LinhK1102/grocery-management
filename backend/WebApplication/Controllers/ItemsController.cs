using BusinessObjects.Entities;
using GroceryWebApp.Models.Dto;
using GroceryWebApp.Service;
using GroceryWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroceryWebApp.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ProductApiService _productApiService;

        public ItemsController(ProductApiService productApiService)
        {
            _productApiService = productApiService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int productId)
        {
            var product = await _productApiService.GetByIdAsync(productId);
            if (product == null) return NotFound();

            ViewBag.Product = product;

            var model = new List<ItemDto>
            {
                new ItemDto
                {
                    ProductId = productId,
                    Barcode = product.BarcodeValue,
                    ImportedDate = DateTime.Today,
                    ManufactureDate = DateTime.Today,
                    ExpiryDate = DateTime.Today.AddMonths(6)
                }
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> CreateMultiple(int productId, string barcode, List<ItemCreateDto> items)
        {
            // Gán ProductId và Barcode cho tất cả items
            foreach (var item in items)
            {
                item.ProductId = productId;
                item.Barcode = barcode;
            }

            // Gửi 1 request duy nhất
            var status = await _productApiService.CreateMultipleItemsAsync(items);

            if (!status)
            {
                TempData["Error"] = "Some items failed to create.";
            }

            return RedirectToAction("Details", "Product", new { id = productId });
        }


    }
}
