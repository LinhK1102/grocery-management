using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.Mvc;
using GroceryWebApp.Models.Dto;
using Repositories.Interfaces;
using System.Threading.Tasks;

namespace GroceryAPI.Controllers.OData
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ODataController
    {
        private readonly IProductRepository _repo;

        public ProductsController(IProductRepository repo)
        {
            _repo = repo;
        }

        [EnableQuery(PageSize = 100)]
        [HttpGet("search-odata")]
        public IActionResult Search()
        {
            var query = _repo.GetQueryable(); // Trả IQueryable<Product>

            var result = query.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                CategoryId = p.CategoryId,
                SupplierId = p.SupplierId,
                UnitsInStock = p.UnitsInStock,
                UnitPrice = p.UnitPrice,
                BarcodeValue = p.BarcodeValue,
                ExpiryDuration = p.ExpiryDuration
            });

            return Ok(result);
        }

    }
}
