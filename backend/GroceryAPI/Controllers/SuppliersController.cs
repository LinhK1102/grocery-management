using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Utility.Common;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/suppliers")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierController(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        [HttpGet("get-all")]
        public IActionResult GetAllSuppliers()
        {
            var suppliers = _supplierRepository.GetAllSuppliers();
            return Ok(SystemStatus.Success(suppliers, "Suppliers retrieved successfully."));
        }

        [HttpGet("get-by-id/{id}")]
        public IActionResult GetSupplierById(int id)
        {
            var supplier = _supplierRepository.GetSupplierById(id);
            return supplier == null
                ? NotFound(SystemStatus.Fail($"Supplier with ID {id} not found."))
                : Ok(SystemStatus.Success(supplier, "Supplier found."));
        }

        [HttpPost("create")]
        public IActionResult CreateSupplier([FromBody] Supplier s)
        {
            _supplierRepository.CreateSupplier(s);
            return CreatedAtAction(nameof(GetSupplierById), new { id = s.SupplierId },
                SystemStatus.Success(s, "Supplier created successfully."));
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateSupplier(int id, [FromBody] Supplier s)
        {
            if (id != s.SupplierId)
                return BadRequest(SystemStatus.Fail("Supplier ID mismatch."));

            _supplierRepository.UpdateSupplier(s);
            return Ok(SystemStatus.Success(s, "Supplier updated successfully."));
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteSupplier(int id)
        {
            var result = _supplierRepository.DeleteSupplierWithDependencyCheck(id);
            return result
                ? Ok(SystemStatus.Success($"Supplier with ID {id} deleted successfully."))
                : BadRequest(SystemStatus.Fail("Cannot delete supplier with existing dependencies."));
        }
    }
}
