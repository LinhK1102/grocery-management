using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierController(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        [HttpGet("GetAllSuppliers")]
        public IActionResult GetAllSuppliers() => Ok(_supplierRepository.GetAllSuppliers());

        [HttpGet("GetSupplierById/{id}")]
        public IActionResult GetSupplierById(int id) => Ok(_supplierRepository.GetSupplierById(id));

        [HttpPost("CreateSupplier")]
        public IActionResult CreateSupplier([FromBody] Supplier s)
        {
            _supplierRepository.CreateSupplier(s);
            return CreatedAtAction(nameof(GetSupplierById), new { id = s.SupplierId }, s);
        }

        [HttpPut("UpdateSupplier/{id}")]
        public IActionResult UpdateSupplier(int id, Supplier s)
        {
            if (id != s.SupplierId) return BadRequest();
            _supplierRepository.UpdateSupplier(s);
            return NoContent();
        }

        [HttpDelete("DeleteSupplier/{id}")]
        public IActionResult DeleteSupplier(int id)
        {
            var result = _supplierRepository.DeleteSupplierWithDependencyCheck(id);
            return result ? NoContent() : BadRequest("Cannot delete supplier with existing dependencies.");
        }

        
    }

}
