using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Repositories.Repositories;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarcodeController : Controller
    {
        private readonly IBarcodeRepository _barcodeRepo;

        public BarcodeController(IBarcodeRepository barcodeRepo)
        {
            _barcodeRepo = barcodeRepo;
        }

        [HttpGet("search/{barcode}")]
        public async Task<ActionResult<UpcProductResponse>> SearchProduct(string barcode)
        {
            var result = await _barcodeRepo.GetProductInfoFromApiAsync(barcode);
            if (result == null || !result.Status)
                return NotFound("Không tìm thấy sản phẩm.");

            return Ok(result);
        }
        [HttpGet("scan/{barcode}")]
        public async Task<IActionResult> ScanBarcode(string barcode)
        {
            var product = await _barcodeRepo.GetOrCreateProductByBarcodeAsync(barcode);
            if (product == null)
                return NotFound("Barcode not found and API has no result.");

            return Ok(product);
        }

    }
}
