using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Utility.Common;
using System.Collections.Generic;

namespace GroceryAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repo;

        public ProductsController(IProductRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("get-all")]
        public IActionResult GetAllProduct()
        {
            var products = _repo.GetAllProduct();
            return Ok(SystemStatus.Success(products, "All products retrieved."));
        }

        [HttpGet("get-by-id/{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _repo.GetProductById(id);
            return product == null
                ? NotFound(SystemStatus.Fail($"Product with ID {id} not found."))
                : Ok(SystemStatus.Success(product, "Product retrieved."));
        }

        [HttpPost("create")]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            _repo.AddProduct(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId },
                SystemStatus.Success(product, "Product created successfully."));
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.ProductId)
                return BadRequest(SystemStatus.Fail("Mismatched product ID."));

            _repo.UpdateProduct(product);
            return Ok(SystemStatus.Success(product, "Product updated successfully."));
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            _repo.DeleteProduct(id);
            return Ok(SystemStatus.Success($"Product with ID {id} deleted successfully."));
        }

        [HttpGet("low-stock")]
        public IActionResult GetLowStockProducts([FromQuery] int threshold = 30)
        {
            var products = _repo.GetLowStockProducts(threshold);
            return Ok(SystemStatus.Success(products, $"Products with stock below {threshold} retrieved."));
        }

        [HttpGet("scan/{barcode}")]
        public IActionResult GetProductByBarcode(string barcode)
        {
            var product = _repo.GetProductByBarcode(barcode);
            return product == null
                ? NotFound(SystemStatus.Fail($"No product found with barcode {barcode}."))
                : Ok(SystemStatus.Success(product, "Product found by barcode."));
        }

        [HttpPost("scan-adjust-stock")]
        public IActionResult AdjustStock([FromBody] BarcodeActionRequest request)
        {
            var product = _repo.GetProductByBarcode(request.Barcode);
            if (product == null)
                return NotFound(SystemStatus.Fail("Product not found."));

            if (request.Action != "sell" && request.Action != "receive")
                return BadRequest(SystemStatus.Fail("Invalid action. Use 'sell' or 'receive'."));

            _repo.AdjustStock(request.Barcode, request.Action, request.Quantity);
            return Ok(SystemStatus.Success(product, $"Stock adjusted by {request.Action}."));
        }

        [HttpGet("supplier-products/{supplierId}")]
        public IActionResult GetSupplierProductList(int supplierId)
        {
            var list = _repo.GetSupplierProductList(supplierId);
            return Ok(SystemStatus.Success(list, $"Products from supplier ID {supplierId} retrieved."));
        }

        public class BarcodeActionRequest
        {
            public string Barcode { get; set; }
            public string Action { get; set; }  // "sell" or "receive"
            public int Quantity { get; set; }
        }
    }
}
