using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Utility.Common;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using GroceryAPI.Hubs;
using BusinessObjects.DTOs;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models.Dto;
namespace GroceryAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repo;
        private readonly IItemRepository _itemRepo;
        private readonly IHubContext<NotificationHubs> _notificationHub; 

        public ProductsController(IProductRepository repo, IHubContext<NotificationHubs> notificationHub, IItemRepository itemRepo)
        {
            _repo = repo;
            _notificationHub = notificationHub;
            _itemRepo = itemRepo;
        }

        [HttpGet("get-all")]
        public IActionResult GetAllProduct()
        {
            var products = _repo.GetAllProduct();
            List<ProductDto> productDtos = new List<ProductDto>();
            //for (var product in products)
            //{

            //}
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
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto product)
        {
            if (id != product.ProductId)
                return BadRequest(SystemStatus.Fail("Mismatched product ID."));

            _repo.UpdateProduct(product);

            // Notify clients about the product update
            await _notificationHub.Clients.All.SendAsync("ProductUpdated", product);

            return Ok(SystemStatus.Success(product, "Product updated successfully."));
        }


        [HttpDelete("delete/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            _repo.DeleteProduct(id);
            // Notify clients about the product deletion
            _notificationHub.Clients.All.SendAsync("ProductDeleted", id);
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

            if (request.Action != UtitlityConstant.Item_Action_Sell && request.Action != UtitlityConstant.Item_Action_Restock)
                return BadRequest(SystemStatus.Fail($"Invalid action. Use {UtitlityConstant.Item_Action_Sell} or {UtitlityConstant.Item_Action_Restock}."));

            var status = _repo.AdjustStock(request.Barcode, request.Action, request.Quantity);
            // Notify clients about the stock adjustment
            _notificationHub.Clients.All.SendAsync($"StockAdjusted {status}", new
            {
                Barcode = request.Barcode,
                Action = request.Action,
                Quantity = request.Quantity
            });
            return Ok(SystemStatus.Success(product, $"Stock adjusted by {request.Action}."));
        }

        [HttpGet("supplier-products/{supplierId}")]
        public IActionResult GetSupplierProductList(int supplierId)
        {
            var list = _repo.GetSupplierProductList(supplierId);
            return Ok(SystemStatus.Success(list, $"Products from supplier ID {supplierId} retrieved."));
        }

        [HttpGet("category-products/{categoryId}")]
        public IActionResult GetCategoryProductList(int categoryId)
        {
            var list = _repo.GetSupplierProductList(categoryId);
            return Ok(SystemStatus.Success(list, $"Products from supplier ID {categoryId} retrieved."));
        }
        public class BarcodeActionRequest
        {
            public string Barcode { get; set; }
            public int Action { get; set; }  // "sell" or "receive"
            public int Quantity { get; set; }
        }

        [HttpPost("items-create")]
        public IActionResult CreateItems([FromBody] Item item)
        {
            var productExists = _repo.GetProductById(item.ProductId);
            if (productExists == null)
            {
                return BadRequest("Product not exist!");
            }

            _itemRepo.CreateItem(item);
            return CreatedAtAction(nameof(GetProductById), new { id = item.ProductId },
                SystemStatus.Success(item, "Items created successfully."));
        }
    }
}
