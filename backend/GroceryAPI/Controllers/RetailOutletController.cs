using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Utility.Common;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/retail-outlets")]
    public class RetailOutletController : ControllerBase
    {
        private readonly IRetailOutletRepository _repo;

        public RetailOutletController(IRetailOutletRepository repo) => _repo = repo;

        [HttpGet("get-all")]
        public IActionResult GetAllRetailOutlets()
        {
            var outlets = _repo.GetAllRetailOutlets();
            return Ok(SystemStatus.Success(outlets, "Retail outlets retrieved."));
        }

        [HttpPost("create")]
        public IActionResult CreateRetailOutlet([FromBody] RetailOutlet ro)
        {
            var created = _repo.CreateRetailOutlet(ro);
            return CreatedAtAction(nameof(GetRetailOutletById), new { id = created.RetailOutletId },
                SystemStatus.Success(created, "Retail outlet created successfully."));
        }

        [HttpGet("get-by-id/{id}")]
        public IActionResult GetRetailOutletById(int id)
        {
            var outlet = _repo.GetRetailOutletById(id);
            return outlet == null
                ? NotFound(SystemStatus.Fail($"Retail outlet with ID {id} not found."))
                : Ok(SystemStatus.Success(outlet, "Retail outlet found."));
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateRetailOutlet(int id, [FromBody] RetailOutlet ro)
        {
            if (id != ro.RetailOutletId)
                return BadRequest(SystemStatus.Fail("Mismatched retail outlet ID."));

            _repo.UpdateRetailOutlet(ro);
            return Ok(SystemStatus.Success(ro, "Retail outlet updated successfully."));
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteRetailOutlet(int id)
        {
            _repo.DeleteRetailOutlet(id);
            return Ok(SystemStatus.Success($"Retail outlet with ID {id} deleted successfully."));
        }
    }
}
