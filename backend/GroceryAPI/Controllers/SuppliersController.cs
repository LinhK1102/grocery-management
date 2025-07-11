using AutoMapper;
using BusinessObjects.DTOs;
using BusinessObjects.Entities;
using GroceryAPI.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Repositories.Interfaces;
using Utility.Common;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/suppliers")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IHubContext<NotificationHubs> _notificationHub;
        private readonly IMapper _mapper;

        public SupplierController(ISupplierRepository supplierRepository, IMapper mapper, IHubContext<NotificationHubs> notificationHub)
        {
            _supplierRepository = supplierRepository;
            _notificationHub = notificationHub;
            _mapper = mapper;
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
        public IActionResult UpdateSupplier(int id, [FromBody] UpdateSupplierDto s)
        {
            if (id != s.SupplierId)
            {
                _notificationHub.Clients.All.SendAsync("ReceiveNotification", $"✅ Supplier '{s.SupplierName}' updated fail.");
                return BadRequest(SystemStatus.Fail("Supplier ID mismatch."));
            }
            var supplier = _mapper.Map<Supplier>(s);
            var result = _supplierRepository.UpdateSupplier(supplier);
            if (result == null)
            {
                _notificationHub.Clients.All.SendAsync("ReceiveNotification", $"✅ Supplier '{s.SupplierName}' updated fail.");
                return NotFound(SystemStatus.Fail($"Supplier with ID {id} not found."));
            }

            _notificationHub.Clients.All.SendAsync("ReceiveNotification", $"✅ Supplier '{s.SupplierName}' updated successfully.");

            return Ok(SystemStatus.Success(result, "Supplier updated successfully."));
        }


        [HttpDelete("delete/{id}")]
        public IActionResult DeleteSupplier(int id)
        {
            var result = _supplierRepository.DeleteSupplierWithDependencyCheck(id);
            if (result) _notificationHub.Clients.All.SendAsync("ReceiveNotification", "Supplier updated successfully.");
            else _notificationHub.Clients.All.SendAsync("ReceiveNotification", "Supplier updated successfully.");

            return result
                ? Ok(SystemStatus.Success($"Supplier with ID {id} deleted successfully."))
                : BadRequest(SystemStatus.Fail("Cannot delete supplier with existing dependencies."));
        }
    }
}
