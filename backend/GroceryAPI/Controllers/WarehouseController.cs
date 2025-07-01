using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Utility.Common;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/warehouses")]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseRepository _repo;

        public WarehouseController(IWarehouseRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("get-all")]
        public IActionResult GetAllWarehouses()
        {
            var warehouses = _repo.GetAllWarehouses();
            return Ok(SystemStatus.Success(warehouses, "Warehouse list retrieved successfully."));
        }

        [HttpPost("create")]
        public IActionResult CreateWarehouse([FromBody] Warehouse wh)
        {
            _repo.CreateWarehouse(wh);
            return CreatedAtAction(nameof(GetWarehouseById), new { id = wh.WarehouseId },
                SystemStatus.Success(wh, "Warehouse created successfully."));
        }

        [HttpGet("get-by-id/{id}")]
        public IActionResult GetWarehouseById(int id)
        {
            var warehouse = _repo.GetWarehouseById(id);
            return warehouse == null
                ? NotFound(SystemStatus.Fail($"Warehouse with ID {id} not found."))
                : Ok(SystemStatus.Success(warehouse, "Warehouse found."));
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateWarehouse(int id, [FromBody] Warehouse wh)
        {
            if (id != wh.WarehouseId)
                return BadRequest(SystemStatus.Fail("Warehouse ID mismatch."));

            _repo.UpdateWarehouse(wh);
            return Ok(SystemStatus.Success(wh, "Warehouse updated successfully."));
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteWarehouse(int id)
        {
            _repo.DeleteWarehouse(id);
            return Ok(SystemStatus.Success($"Warehouse with ID {id} deleted successfully."));
        }
    }
}
