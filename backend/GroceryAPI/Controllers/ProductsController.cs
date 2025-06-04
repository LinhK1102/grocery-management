using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Repositories.Repositories;
using System.Collections.Generic;

namespace GroceryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repo;

        public ProductsController(IProductRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetAllProduct")]
        public ActionResult<IEnumerable<Product>> GetAllProduct()
        {
            var products = _repo.GetAllProduct();
            return Ok(products);
        }

        [HttpGet("GetProductById/{id}")]
        public ActionResult<Product> GetProductById(int id)
        {
            var product = _repo.GetProductById(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost("CreateProduct")]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            _repo.AddProduct(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
        }

        [HttpPut("UpdateProduct/{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.ProductId) return BadRequest();

            _repo.UpdateProduct(product);
            return NoContent();
        }

        [HttpDelete("DeleteProduct/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            _repo.DeleteProduct(id);
            return NoContent();
        }

        [HttpGet("low-stock")]
        public ActionResult<IEnumerable<Product>> GetLowStockProducts(int threshold = 30)
        {
            var products = _repo.GetLowStockProducts(threshold);
            return Ok(products);
        }

        [HttpGet("scan/{barcode}")]
        public ActionResult<Product> GetProductByBarcode(string barcode)
        {
            var product = _repo.GetProductByBarcode(barcode);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost("scan-adjust-stock")]
        public IActionResult AdjustStock([FromBody] BarcodeActionRequest request)
        {
            var product = _repo.GetProductByBarcode(request.Barcode);
            if (product == null) return NotFound();

            if (request.Action != "sell" && request.Action != "receive")
                return BadRequest("Unknown action");

            _repo.AdjustStock(request.Barcode, request.Action, request.Quantity);
            return Ok(product);
        }

        [HttpGet("list-products/{supplierId}")]
        public IActionResult GetSupplierProductList(int supplierId) => Ok(_repo.GetSupplierProductList(supplierId));

        public class BarcodeActionRequest
        {
            public string Barcode { get; set; }
            public string Action { get; set; }  // "sell" or "receive"
            public int Quantity { get; set; }
        }
    }
}
