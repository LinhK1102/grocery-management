using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Repositories.Repositories;
using Utility.Common;

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
            var result = await _barcodeRepo.GetProductInfoFromApiAsync(barcode.Trim());
            if (result == null || !result.Status)
                return NotFound("Không tìm thấy sản phẩm.");

            return Ok(result);
        }
        [HttpGet("scan/{barcode}")]
        public async Task<IActionResult> ScanBarcode(string barcode)
        {
            var product = await _barcodeRepo.GetOrCreateProductByBarcodeAsync(barcode.Trim());
            if (product == null)
                return NotFound("Barcode not found and API has no result.");

            return Ok(SystemStatus.Success(product, "Barcode found."));
        }

    }
}
