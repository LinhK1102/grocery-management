using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseRepository _repo;

        public WarehouseController(IWarehouseRepository repo) => _repo = repo;

        [HttpGet("GetAllWarehouses")]
        public IActionResult GetAllWarehouses() => Ok(_repo.GetAllWarehouses());

        [HttpPost("CreateWarehouse")]
        public IActionResult CreateWarehouse([FromBody] Warehouse wh)
        {
            _repo.CreateWarehouse(wh);
            return Ok();
        }

        [HttpPut("UpdateWarehouse/{id}")]
        public IActionResult UpdateWarehouse(int id, Warehouse wh)
        {
            if (id != wh.WarehouseId) return BadRequest();
            _repo.UpdateWarehouse(wh);
            return NoContent();
        }

        [HttpDelete("DeleteWarehouse/{id}")]
        public IActionResult DeleteWarehouse(int id)
        {
            _repo.DeleteWarehouse(id);
            return NoContent();
        }
    }

}
