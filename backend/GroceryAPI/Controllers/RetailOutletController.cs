using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RetailOutletController : ControllerBase
    {
        private readonly IRetailOutletRepository _repo;

        public RetailOutletController(IRetailOutletRepository repo) => _repo = repo;

        [HttpGet]
        public IActionResult GetAllRetailOutlets() => Ok(_repo.GetAllRetailOutlets());

        [HttpPost("CreateRetailOutlet")]
        public IActionResult CreateRetailOutlet([FromBody] RetailOutlet ro)
        {
            _repo.CreateRetailOutlet(ro);
            return Ok();
        }

        [HttpPut("UpdateRetailOutlet/{id}")]
        public IActionResult UpdateRetailOutlet(int id, RetailOutlet ro)
        {
            if (id != ro.RetailOutletId) return BadRequest();
            _repo.UpdateRetailOutlet(ro);
            return NoContent();
        }

        [HttpDelete("DeleteRetailOutlet/{id}")]
        public IActionResult DeleteRetailOutlet(int id)
        {
            _repo.DeleteRetailOutlet(id);
            return NoContent();
        }
    }
}
