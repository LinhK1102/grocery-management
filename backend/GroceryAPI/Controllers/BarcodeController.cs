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

        [HttpGet("{barcode}")]
        public async Task<ActionResult<UpcProductResponse>> GetProduct(string barcode)
        {
            var result = await _barcodeRepo.GetProductByBarcodeAsync(barcode);
            if (result == null || !result.Status)
                return NotFound("Không tìm thấy sản phẩm.");

            return Ok(result);
        }
    }
}
