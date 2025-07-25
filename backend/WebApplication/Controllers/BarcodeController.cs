using GroceryWebApp.Service;
using GroceryWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Threading.Tasks;

namespace GroceryWebApp.Controllers
{
    [Route("[controller]")]
    public class BarcodeController : Controller
    {
        private readonly ProductApiService _productApiService;
        private readonly BarcodeApiService _barcodeApiService;

        public BarcodeController(ProductApiService productApiService, BarcodeApiService barcodeApiService)
        {
            _productApiService = productApiService;
            _barcodeApiService = barcodeApiService;
        }

        [HttpGet("Scan")]
        public IActionResult Scan()
        {
            return View(); // Views/Barcode/Scan.cshtml
        }

        //[HttpPost]
        //public async Task<IActionResult> HandleBarcode(string barcodeValue)
        //{
        //    if (string.IsNullOrWhiteSpace(barcodeValue))
        //    {
        //        TempData["Error"] = "No barcode was scanned.";
        //        return RedirectToAction("Scan");
        //    }

        //    var productUpc = await _barcodeApiService.ScanOrCreateProductAsync(barcodeValue);

        //    if (productUpc != null)
        //        return RedirectToAction(nameof(Details), "Product", new { id = productUpc.ProductId });

        //    TempData["ScannedBarcode"] = barcodeValue;
        //    return RedirectToAction("Create", "Product");
        //}

        [HttpPost]
        public async Task<IActionResult> HandleBarcode(string barcodeValue)
        {
            if (!string.IsNullOrEmpty(barcodeValue))
            {
                var productUpc = await _barcodeApiService.ScanOrCreateProductAsync(barcodeValue);

                TempData["Message"] = $"Đã quét mã: {barcodeValue}";
                if (productUpc != null)
                    return RedirectToAction(nameof(Details), "Product", new { id = productUpc.ProductId });
            }

            TempData["Error"] = "Không nhận được mã barcode";
            return RedirectToAction("ScanBarcode");
        }

    }

}
